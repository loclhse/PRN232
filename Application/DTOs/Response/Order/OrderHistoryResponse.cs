using Domain.Enums;

namespace Application.DTOs.Response.Order
{
    public class OrderHistoryResponse
    {
        public Guid Id { get; set; }
        public OrderStatus Status { get; set; }
        public string Note { get; set; } = string.Empty;
        public string ChangedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}