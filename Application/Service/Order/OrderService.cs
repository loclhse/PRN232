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
            // KHỐI LOGIC TÍNH TOÁN (BACKEND TỰ XỬ LÝ)
            // ==========================================

            // 1. Tính tổng tiền hàng (TotalAmount) từ danh sách OrderDetails
            order.TotalAmount = request.OrderDetails.Sum(item => item.Quantity * item.UnitPrice);

            // 2. Tính tiền giảm giá (DiscountAmount)
            if (request.VoucherId.HasValue)
            {
                // TODO: Mốt bạn gọi _unitOfWork.VoucherRepository để check logic giảm giá ở đây
                // Tạm thời hardcode nếu có voucher thì giảm 10% (ví dụ)
                order.DiscountAmount = order.TotalAmount * 0.1m;
            }
            else
            {
                order.DiscountAmount = 0;
            }

            // 3. Tính tiền cuối cùng (FinalAmount)
            order.FinalAmount = order.TotalAmount - order.DiscountAmount + request.ShippingFee;

            // ==========================================

            await _unitOfWork.OrderRepository.AddAsync(order);

            // Khởi tạo History lần đầu
            var history = new OrderHistory
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                Status = OrderStatus.Pending,
                Note = "Khởi tạo đơn hàng mới",
                ChangedBy = "System",
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Repository<OrderHistory>().AddAsync(history);

            // ==========================================
            //  TẠO BẢN GHI PAYMENT TẠI ĐÂY
            // ==========================================
            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id, // Gắn Payment này với ID của đơn hàng vừa tạo
                PaymentMethod = request.PaymentMethod, // Lấy phương thức (VD: "COD", "VNPay") từ Request

                // Nếu là COD thì Pending (chờ thu tiền), nếu thanh toán Online thì WaitingForPayment (chờ quẹt thẻ)
                Status = request.PaymentMethod == "COD" ? "Pending" : "WaitingForPayment",

                // Lưu ý quan trọng: Số tiền khách phải thanh toán là FinalAmount (đã cộng ship, trừ voucher)
                Amount = order.FinalAmount,

                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Repository<Payment>().AddAsync(payment);
            // ==========================================

            // Lưu tất cả Order, OrderDetails, OrderHistory, và Payment trong 1 Transaction duy nhất
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<OrderResponse>(order);
        }

        public async Task<OrderResponse?> UpdateOrderStatusAsync(Guid id, OrderStatus newStatus)
        {
            // Cần include OrderDetails và OrderHistories để khi trả về FE có data mới nhất
            var order = await _unitOfWork.OrderRepository.GetFirstOrDefaultAsync(
                filter: o => o.Id == id && !o.IsDeleted,
                includeProperties: "OrderDetails,OrderHistories" //
            );

            if (order == null) return null;

            order.CurrentStatus = newStatus;
            order.UpdatedAt = DateTime.UtcNow;

            // Tự động sinh OrderHistory mới
            var history = new OrderHistory
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                Status = newStatus,
                Note = $"Hệ thống cập nhật trạng thái thành: {newStatus}",
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