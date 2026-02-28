using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Request.Inventory
{
    public class UpdateInventoryRequest
    {
        [Required(ErrorMessage = "Quantity is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be non-negative")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Minimum stock level is required")]
        [Range(0, int.MaxValue, ErrorMessage = "MinStockLevel must be non-negative")]
        public int MinStockLevel { get; set; }
    }
}
