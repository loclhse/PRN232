using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int ItemId { get; set; } // ProductId or GiftBoxId depending on ItemType
        
        [Required]
        [MaxLength(50)]
        public string ItemType { get; set; } = "PRODUCT"; // PRODUCT or GIFTBOX

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; } = null!;
        
        // Optional navigation properties - might be tricky with single ItemId column
        // But useful to have mapped slightly differently or just logical references
        [ForeignKey("ItemId")]
        public virtual Product? Product { get; set; } // Only if ItemType == PRODUCT

        [ForeignKey("ItemId")]
        public virtual GiftBox? GiftBox { get; set; } // Only if ItemType == GIFTBOX
    }
}
