using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    // Note: Composite Key (GiftBoxId, ProductId) needs configuration in DbContext (Fluent API)
    public class BoxComponent : BaseEntity
    {
        public Guid GiftBoxId { get; set; }
        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        [ForeignKey("GiftBoxId")]
        public virtual GiftBox GiftBox { get; set; } = null!;

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;
    }
}
