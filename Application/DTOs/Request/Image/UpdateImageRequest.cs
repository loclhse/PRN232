using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Request.Image
{
    public class UpdateImageRequest
    {
        [Required]
        [MaxLength(500)]
        public string Url { get; set; } = string.Empty;

        public bool IsMain { get; set; }

        public int SortOrder { get; set; }
    }
}