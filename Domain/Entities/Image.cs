using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Image
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string Url { get; set; } = string.Empty;

        public bool IsMain { get; set; }
        public int SortOrder { get; set; }

        // Polymorphic relationship simplified
        public int OwnerId { get; set; }
        
        [MaxLength(50)]
        public string OwnerType { get; set; } = string.Empty; // "Product" or "GiftBox"
    }
}
