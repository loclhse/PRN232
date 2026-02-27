using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Request.Order
{
    public class CreateOrderDetailRequest
    {
        [Required]
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}