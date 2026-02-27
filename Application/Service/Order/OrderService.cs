using Application.DTOs.Request.Order;
using Application.DTOs.Response.Order;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.IUnitOfWork;

namespace Application.Service.Order
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<OrderResponse?> GetOrderByIdAsync(Guid id)
        {
            var order = await _unitOfWork.OrderRepository.GetFirstOrDefaultAsync(
                filter: o => o.Id == id && !o.IsDeleted,
                includeProperties: "OrderDetails,OrderHistories" // Gọi ra các bảng con
            );
            return _mapper.Map<OrderResponse>(order);
        }

        public async Task<IEnumerable<OrderResponse>> GetAllOrdersAsync()
        {
            var orders = await _unitOfWork.OrderRepository.FindAsync(
                filter: o => !o.IsDeleted,
                includeProperties: "OrderDetails,OrderHistories" // Gọi ra các bảng con
            );
            return _mapper.Map<IEnumerable<OrderResponse>>(orders);
        }

        public async Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request)
        {
            // AutoMapper sẽ map các thông tin cơ bản từ Request sang Order
            // Lưu ý: Lúc này các chi tiết đơn hàng (OrderDetails) đang có UnitPrice = 0 vì FE không truyền lên
            var order = _mapper.Map<Domain.Entities.Order>(request);

            // =======================================================
            // KHỐI 1: TÍNH TOÁN TIỀN (BẢO MẬT: LẤY GIÁ TỪ DATABASE)
            // =======================================================

            // 1. Lấy danh sách ID của tất cả sản phẩm mà khách đặt
            var productIds = request.OrderDetails.Select(x => x.ProductId).ToList();

            // 2. Gom thành 1 câu query duy nhất để lấy thông tin các sản phẩm này từ DB (Tối ưu hiệu năng)
            var productRepo = _unitOfWork.Repository<Domain.Entities.Product>();
            var productsInDb = await productRepo.FindAsync(p => productIds.Contains(p.Id));

            order.TotalAmount = 0; // Khởi tạo tổng tiền hàng

            // 3. Duyệt qua từng dòng chi tiết đơn hàng để gán giá chuẩn
            foreach (var detail in order.OrderDetails)
            {
                var product = productsInDb.FirstOrDefault(p => p.Id == detail.ProductId);
                if (product == null)
                    throw new Exception($"Sản phẩm với ID {detail.ProductId} không tồn tại hoặc đã bị xóa!");

                // Tự động gán giá gốc của product từ Database
                detail.UnitPrice = product.Price;

                // Cộng dồn vào tổng tiền hàng của đơn
                order.TotalAmount += (detail.Quantity * detail.UnitPrice);
            }

            order.DiscountAmount = 0; // Khởi tạo tiền giảm giá

            // =======================================================
            // KHỐI 2: TỰ ĐỘNG TÍNH PHÍ SHIP Ở BACKEND
            // =======================================================
            // Logic: Tổng tiền hàng >= 500k thì Freeship (0đ), ngược lại thu 30k
            decimal backendShippingFee = order.TotalAmount >= 500000 ? 0 : 30000;

            // =======================================================
            // KHỐI 3: XỬ LÝ VOUCHER (KIỂM TRA CHẶT CHẼ)
            // =======================================================
            if (request.VoucherId.HasValue)
            {
                var repoVoucher = _unitOfWork.Repository<Domain.Entities.Voucher>();
                var voucher = await repoVoucher.GetByIdAsync(request.VoucherId.Value);

                // Các lớp phòng thủ kiểm tra tính hợp lệ của Voucher
                if (voucher == null)
                    throw new Exception("Voucher không tồn tại!");
                if (!voucher.IsActive || voucher.EndDate < DateTime.UtcNow)
                    throw new Exception("Voucher đã hết hạn hoặc bị vô hiệu hóa!");
                if (order.TotalAmount < voucher.MinOrderValue)
                    throw new Exception($"Đơn hàng chưa đạt giá trị tối thiểu ({voucher.MinOrderValue}đ) để áp dụng voucher này!");
                if (voucher.UsageLimit <= 0)
                    throw new Exception("Voucher này đã hết lượt sử dụng!");

                // Tính toán số tiền được giảm
                if (voucher.DiscountType == "PERCENT")
                {
                    decimal calculatedDiscount = order.TotalAmount * (voucher.Value / 100);
                    // Dùng Math.Min để không bao giờ giảm vượt quá số tiền tối đa (MaxDiscountAmount)
                    order.DiscountAmount = voucher.MaxDiscountAmount.HasValue
                        ? Math.Min(calculatedDiscount, voucher.MaxDiscountAmount.Value)
                        : calculatedDiscount;
                }
                else
                {
                    // Giảm tiền mặt trực tiếp
                    order.DiscountAmount = voucher.Value;
                }

                // Trừ đi 1 lượt sử dụng của Voucher và đánh dấu cần Update
                voucher.UsageLimit -= 1;
                repoVoucher.Update(voucher);
            }

            // =======================================================
            // KHỐI 4: TỔNG KẾT TIỀN & RẼ NHÁNH TRẠNG THÁI
            // =======================================================

            // Tính tiền khách phải trả cuối cùng
            order.FinalAmount = order.TotalAmount - order.DiscountAmount + backendShippingFee;

            // [LUỒNG TỰ ĐỘNG]: Nếu COD -> Tự động Confirmed. Nếu Online -> Pending chờ thanh toán
            order.CurrentStatus = request.PaymentMethod == "COD" ? OrderStatus.Confirmed : OrderStatus.Pending;

            // Đưa Order vào danh sách chờ thêm mới
            await _unitOfWork.OrderRepository.AddAsync(order);

            // =======================================================
            // KHỐI 5: LƯU VẾT LỊCH SỬ VÀ TẠO GIAO DỊCH THANH TOÁN
            // =======================================================

            // Khởi tạo History lần đầu ứng với trạng thái tự động
            var history = new OrderHistory
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                Status = order.CurrentStatus,
                Note = request.PaymentMethod == "COD" ? "Hệ thống tự động xác nhận đơn COD" : "Khởi tạo đơn hàng chờ thanh toán online",
                ChangedBy = "System",
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Repository<OrderHistory>().AddAsync(history);

            // Tạo bản ghi lưu giao dịch tiền bạc (PAYMENT)
            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                PaymentMethod = request.PaymentMethod,
                Status = request.PaymentMethod == "COD" ? "Pending" : "WaitingForPayment",
                Amount = order.FinalAmount,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Repository<Payment>().AddAsync(payment);

            // =======================================================
            // KHỐI 6: COMMIT LƯU DATABASE & TRẢ VỀ RESPONSE
            // =======================================================

            // Lưu tất cả mọi thứ (Order, OrderDetails, History, Payment, Voucher update) 
            // trong 1 Transaction an toàn duy nhất
            await _unitOfWork.SaveChangesAsync();

            // Ánh xạ ra Response để trả về cho Frontend
            var response = _mapper.Map<OrderResponse>(order);

            // Ghi đè lại 2 trường này để FE hiển thị đúng dữ liệu thực tế đã xử lý
            response.PaymentMethod = request.PaymentMethod;
            response.ShippingFee = backendShippingFee;

            return response;
        }

        public async Task<OrderResponse?> UpdateOrderStatusAsync(Guid id, OrderStatus newStatus)
        {
            var order = await _unitOfWork.OrderRepository.GetFirstOrDefaultAsync(
                filter: o => o.Id == id && !o.IsDeleted,
                includeProperties: "OrderDetails,OrderHistories"
            );

            if (order == null) return null;

            // Nếu trạng thái không thay đổi thì không làm gì cả
            if (order.CurrentStatus == newStatus)
                return _mapper.Map<OrderResponse>(order);

            order.CurrentStatus = newStatus;
            order.UpdatedAt = DateTime.UtcNow;

            string historyNote = $"Hệ thống cập nhật trạng thái thành: {newStatus}";

            // Lấy thông tin Payment của đơn hàng này để cập nhật trạng thái tiền bạc
            var paymentRepo = _unitOfWork.Repository<Payment>();
            var paymentList = await paymentRepo.FindAsync(p => p.OrderId == order.Id);
            var payment = paymentList.FirstOrDefault();

            // ========================================================
            // XỬ LÝ LOGIC CHO TỪNG NHÁNH TRẠNG THÁI
            // ========================================================
            switch (newStatus)
            {
                case OrderStatus.Shipping:
                    // Bước 5: Đi giao -> Vì bỏ TrackingNumber nên chỉ cần ghi chú đơn giản
                    historyNote = $"Bắt đầu giao hàng (Mã đơn: {order.OrderNumber}).";
                    break;

                case OrderStatus.Delivered:
                    // Nhánh 3: Giao thành công -> Đổi trạng thái thanh toán thành Success
                    if (payment != null && payment.Status != "Success")
                    {
                        payment.Status = "Success";
                        paymentRepo.Update(payment);
                    }
                    historyNote = "Giao hàng thành công. Đã thu tiền.";
                    break;

                case OrderStatus.Cancelled:
                    // Nhánh 1: Bị hủy -> Nếu là COD thì Cancel, Online thì Failed
                    if (payment != null)
                    {
                        payment.Status = payment.PaymentMethod == "COD" ? "Cancelled" : "Failed";
                        paymentRepo.Update(payment);
                    }

                    // Hoàn trả lại 1 lượt dùng Voucher cho hệ thống
                    if (order.VoucherId.HasValue)
                    {
                        var voucherRepo = _unitOfWork.Repository<Domain.Entities.Voucher>();
                        var voucher = await voucherRepo.GetByIdAsync(order.VoucherId.Value);
                        if (voucher != null)
                        {
                            voucher.UsageLimit += 1;
                            voucherRepo.Update(voucher);
                        }
                    }
                    historyNote = "Đơn hàng đã bị hủy.";
                    break;

                case OrderStatus.Returned:
                    // Nhánh 4: Boom hàng / Hoàn trả -> Không thu được tiền nên Payment Failed
                    if (payment != null)
                    {
                        payment.Status = "Failed";
                        paymentRepo.Update(payment);
                    }
                    historyNote = "Khách boom hàng / Trả hàng. Giao thất bại.";
                    break;

                case OrderStatus.Confirmed:
                case OrderStatus.Processing:
                    // Các trạng thái này chỉ cần cập nhật chữ (đã cấu hình mặc định ở trên)
                    break;
            }
            // ========================================================

            // Tự động sinh OrderHistory mới để lưu vết
            var history = new OrderHistory
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                Status = newStatus,
                Note = historyNote,
                ChangedBy = "System",
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Repository<OrderHistory>().AddAsync(history);

            _unitOfWork.OrderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<OrderResponse>(order);
        }

        public async Task<bool> DeleteOrderAsync(Guid id)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(id);
            if (order == null || order.IsDeleted) return false;

            order.IsDeleted = true;
            _unitOfWork.OrderRepository.Update(order);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }
    }
}