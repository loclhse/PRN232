using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Request.Order
{
    public class CreateOrderRequest
    {
        [Required]
        public Guid UserId { get; set; }

        // Bỏ dòng ShippingFee đi vì được hard code trong OrderService
        // public decimal ShippingFee { get; set; } 

        public string? Note { get; set; }

        [Required]
        public string PaymentMethod { get; set; } = "COD";

        public Guid? VoucherId { get; set; }

        [Required]
        [Phone]
        public string ShippingPhone { get; set; } = string.Empty;

        // Đảm bảo có thông tin địa chỉ để giao hàng
        [Required]
        public string ShippingAddress { get; set; } = string.Empty;

        public List<CreateOrderDetailRequest> OrderDetails { get; set; } = new List<CreateOrderDetailRequest>();
    }
}