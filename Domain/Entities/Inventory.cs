using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Domain.Enums;

namespace Domain.Entities
{
    // Note: Composite key (BranchId, ProductId) might be good, or just Id
    public class Inventory : BaseEntity
    {

        public Guid BranchId { get; set; }
        public Guid ProductId { get; set; }

        public int Quantity { get; set; }
        public int MinStockLevel { get; set; }

        public InventoryStatus Status { get; set; } = InventoryStatus.InStock;

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; } = null!;

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;

        public virtual ICollection<InventoryTransaction> Transactions { get; set; } = new List<InventoryTransaction>();
    }
}
