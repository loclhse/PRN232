using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Request.Cart
{
    public class UpdateCartItemRequest
    {
        // Số lượng mới
        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }
}
