using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class PaymentHistory
    {
        [Key]
        public int Id { get; set; }

        public int PaymentId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty;

        public string? RawResponse { get; set; } // JSON from payment gateway

        public string? Note { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [ForeignKey("PaymentId")]
        public virtual Payment Payment { get; set; } = null!;
    }
}
