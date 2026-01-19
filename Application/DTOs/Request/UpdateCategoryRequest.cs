using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Request
{
    public class UpdateCategoryRequest
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public Guid? ParentId { get; set; }
    }
}
