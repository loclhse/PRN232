using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Image : BaseEntity
    {

        [Required]
        [MaxLength(500)]
        public string Url { get; set; } = string.Empty;

        public bool IsMain { get; set; }
        public int SortOrder { get; set; }

        public Guid? ProductId { get; set; }
        public Product? Product { get; set; }
        public Guid? GiftBoxId { get; set; }
        public GiftBox? GiftBox { get; set; }
    }
}
