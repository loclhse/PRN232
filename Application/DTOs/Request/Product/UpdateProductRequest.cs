using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Request.Product
{
    public class UpdateProductRequest
    {
        [Required]
        [MaxLength(50)]
        public string SKU { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0")]
        public decimal Price { get; set; }

        public bool IsActive { get; set; } = true;

        [Required]
        public Guid CategoryId { get; set; }
    }
}
