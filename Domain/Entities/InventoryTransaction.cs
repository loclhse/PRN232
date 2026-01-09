using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class InventoryTransaction
    {
        [Key]
        public int Id { get; set; }

        public int InventoryId { get; set; }

        public int QuantityChange { get; set; } // + or -

        [Required]
        [MaxLength(50)]
        public string TransactionType { get; set; } = string.Empty; // Import, Sale, Return, Damage, Transfer

        [MaxLength(50)]
        public string? ReferenceId { get; set; } // OrderId, ImportReceiptId

        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        [MaxLength(100)]
        public string? CreatedBy { get; set; } // Username or StaffId

        [ForeignKey("InventoryId")]
        public virtual Inventory Inventory { get; set; } = null!;
    }
}
