using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    // Note: Composite key (BranchId, ProductId) might be good, or just Id
    public class Inventory
    {
        [Key]
        public int Id { get; set; }

        public int BranchId { get; set; }
        public int ProductId { get; set; }

        public int Quantity { get; set; }
        public int MinStockLevel { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; } = null!;

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;

        public virtual ICollection<InventoryTransaction> Transactions { get; set; } = new List<InventoryTransaction>();
    }
}
