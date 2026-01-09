using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class OrderHistory
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty;

        public string? Note { get; set; }

        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(100)]
        public string? ChangedBy { get; set; } // Username

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; } = null!;
    }
}
