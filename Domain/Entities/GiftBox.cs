using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class GiftBox : BaseEntity
    {

        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal BasePrice { get; set; }

        public bool IsActive { get; set; } = true;

        public Guid CategoryId { get; set; }
        public Guid GiftBoxComponentConfigId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; } = null!;

        [ForeignKey("GiftBoxComponentConfigId")]
        public virtual GiftBoxComponentConfig ComponentConfig { get; set; } = null!;

        public virtual ICollection<BoxComponent> BoxComponents { get; set; } = new List<BoxComponent>();
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public virtual ICollection<Image> Images { get; set; } = new List<Image>();
    }
}
