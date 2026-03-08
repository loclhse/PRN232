using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Request.GiftBox
{
    public class UpdateGiftBoxRequest
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

        // Null for custom/customize gift boxes
        public Guid? GiftBoxComponentConfigId { get; set; }

        // Nếu null thì không thay đổi, nếu empty thì xóa hết
        public List<BoxComponentItemRequest>? Items { get; set; }

        // Nếu null thì không thay đổi, nếu empty thì xóa hết
        public List<string>? ImageUrls { get; set; }
    }
}
