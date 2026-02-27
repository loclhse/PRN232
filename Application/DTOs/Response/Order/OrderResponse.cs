using Domain.Enums;

namespace Application.DTOs.Response.Order
{
    public class OrderResponse
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public Guid? VoucherId { get; set; }

        // Nhóm Tài chính
        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal FinalAmount { get; set; }

        // Nhóm Trạng thái & Giao hàng
        public OrderStatus CurrentStatus { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        // Thêm PaymentStatus nếu bạn có Enum này, hoặc dùng string
        public string ShippingAddress { get; set; } = string.Empty;
        public string ShippingMethod { get; set; } = string.Empty;
        public string TrackingNumber { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public string Note { get; set; } = string.Empty;

        public List<OrderDetailResponse> OrderDetails { get; set; } = new List<OrderDetailResponse>();
        public List<OrderHistoryResponse> OrderHistories { get; set; } = new List<OrderHistoryResponse>();
    }
}