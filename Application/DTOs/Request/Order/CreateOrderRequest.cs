using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Request.Order
{
    public class CreateOrderRequest
    {
        [Required]
        public Guid UserId { get; set; }

        // Thông tin giao hàng
        [Required]
        public string ShippingAddress { get; set; } = string.Empty;
        public string ShippingMethod { get; set; } = "Standard";

        // Tiền ship (Có thể do FE gọi API bên thứ 3 (GHN, GHTK) truyền vào, hoặc BE tự tính)
        public decimal ShippingFee { get; set; }

        // Thông tin thanh toán & Khuyến mãi
        [Required]
        public string PaymentMethod { get; set; } = "COD";
        public Guid? VoucherId { get; set; }

        public string Note { get; set; } = string.Empty;

        [Required]
        public List<CreateOrderDetailRequest> OrderDetails { get; set; } = new List<CreateOrderDetailRequest>();
    }
}