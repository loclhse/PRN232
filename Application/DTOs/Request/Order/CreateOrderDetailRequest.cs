using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Request.Order
{
    public class CreateOrderDetailRequest
    {
        /// <summary>Optional. One of ProductId or GiftBoxId must be set per line.</summary>
        public Guid? ProductId { get; set; }

        /// <summary>Optional. One of ProductId or GiftBoxId must be set per line.</summary>
        public Guid? GiftBoxId { get; set; }

        [Required]
        public int Quantity { get; set; }
        
        // Price từ CartItem (hiển thị cho user, backend sẽ override từ DB)
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
    }
}