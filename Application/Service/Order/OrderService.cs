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
            var order = _mapper.Map<Domain.Entities.Order>(request);

            // ==========================================
            // KHỐI 1: TÍNH TOÁN TIỀN VÀ XỬ LÝ VOUCHER
            // ==========================================

            // 1. Tính tổng tiền hàng (TotalAmount)
            order.TotalAmount = request.OrderDetails.Sum(item => item.Quantity * item.UnitPrice);
            order.DiscountAmount = 0;

            // 2. TỰ ĐỘNG TÍNH PHÍ SHIP Ở BACKEND (Hóa đơn >= 500k thì freeship, ngược lại 30k)
            decimal backendShippingFee = order.TotalAmount >= 500000 ? 0 : 30000;

            // 3. Áp dụng logic Voucher
            if (request.VoucherId.HasValue)
            {
                var repoVoucher = _unitOfWork.Repository<Domain.Entities.Voucher>();
                var voucher = await repoVoucher.GetByIdAsync(request.VoucherId.Value);

                if (voucher == null)
                    throw new Exception("Voucher không tồn tại!");
                if (!voucher.IsActive || voucher.EndDate < DateTime.UtcNow)
                    throw new Exception("Voucher đã hết hạn hoặc bị vô hiệu hóa!");
                if (order.TotalAmount < voucher.MinOrderValue)
                    throw new Exception($"Đơn hàng chưa đạt giá trị tối thiểu ({voucher.MinOrderValue}đ) để áp dụng voucher này!");
                if (voucher.UsageLimit <= 0)
                    throw new Exception("Voucher này đã hết lượt sử dụng!");

                if (voucher.DiscountType == "PERCENT")
                {
                    decimal calculatedDiscount = order.TotalAmount * (voucher.Value / 100);
                    order.DiscountAmount = voucher.MaxDiscountAmount.HasValue
                        ? Math.Min(calculatedDiscount, voucher.MaxDiscountAmount.Value)
                        : calculatedDiscount;
                }
                else
                {
                    order.DiscountAmount = voucher.Value;
                }

                voucher.UsageLimit -= 1;
                repoVoucher.Update(voucher);
            }

            // 4. Tính tiền cuối cùng (Dùng phí ship tự tính, KHÔNG dùng request.ShippingFee nữa)
            order.FinalAmount = order.TotalAmount - order.DiscountAmount + backendShippingFee;

            // ==========================================
            // KHỐI 2: RẼ NHÁNH TRẠNG THÁI & LƯU DATABASE
            // ==========================================

            // RẼ NHÁNH 2: Nếu COD -> tự động Confirmed. Nếu Online -> Pending chờ thanh toán
            order.CurrentStatus = request.PaymentMethod == "COD" ? OrderStatus.Confirmed : OrderStatus.Pending;

            await _unitOfWork.OrderRepository.AddAsync(order);

            // Khởi tạo History lần đầu với đúng trạng thái tự động
            var history = new OrderHistory
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                Status = order.CurrentStatus, // Lấy đúng Status vừa được rẽ nhánh ở trên
                Note = request.PaymentMethod == "COD" ? "Hệ thống tự động xác nhận đơn COD" : "Khởi tạo đơn hàng chờ thanh toán online",
                ChangedBy = "System",
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Repository<OrderHistory>().AddAsync(history);

            // Tạo bản ghi PAYMENT
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

            // Lưu tất cả trong 1 Transaction
            await _unitOfWork.SaveChangesAsync();

            // Ánh xạ ra Response
            var response = _mapper.Map<OrderResponse>(order);
            response.PaymentMethod = request.PaymentMethod;
            // Ghi đè lại phí ship thực tế để FE hiển thị đúng những gì Backend đã tính
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