using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Category : BaseEntity
    {

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public Guid? ParentId { get; set; }

        [ForeignKey("ParentId")]
        public virtual Category? ParentCategory { get; set; }
        public virtual ICollection<Category> SubCategories { get; set; } = new List<Category>();

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
        public virtual ICollection<GiftBox> GiftBoxes { get; set; } = new List<GiftBox>();
    }
}
