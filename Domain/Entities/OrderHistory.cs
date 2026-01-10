using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class OrderHistory : BaseEntity
    {
        public int OrderId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty;

        public string? Note { get; set; }

        [MaxLength(100)]
        public string? ChangedBy { get; set; } // Username

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; } = null!;
    }
}
