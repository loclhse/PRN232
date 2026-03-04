using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Request.GiftBox
{
    public class CreateGiftBoxRequest
    {
        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Base price must be greater than or equal to 0")]
        public decimal BasePrice { get; set; }

        public bool IsActive { get; set; } = true;

        [Required]
        public Guid CategoryId { get; set; }

        /// Optional. Null for custom/customize gift boxes
        public Guid? GiftBoxComponentConfigId { get; set; }

        public List<BoxComponentItemRequest> Items { get; set; } = new List<BoxComponentItemRequest>();

        public List<string> ImageUrls { get; set; } = new List<string>();
    }

    public class BoxComponentItemRequest
    {
        [Required]
        public Guid ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }
}
