using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Request.Cart
{
    // Request để checkout giỏ hàng sang Order
    public class CheckoutRequest
    {
        // Địa chỉ giao hàng
        [Required(ErrorMessage = "Shipping address is required")]
        [MaxLength(500, ErrorMessage = "Shipping address cannot exceed 500 characters")]
        public string ShippingAddress { get; set; } = string.Empty;

        // Số điện thoại giao hàng
        [Required(ErrorMessage = "Shipping phone is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        [MaxLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        public string ShippingPhone { get; set; } = string.Empty;

        public string? VoucherCode { get; set; }

        [MaxLength(500, ErrorMessage = "Note cannot exceed 500 characters")]
        public string? Note { get; set; }

        // Danh sách CartItem IDs cần checkout (nếu null thì checkout tất cả)
        public List<Guid>? SelectedItemIds { get; set; }
    }
}
