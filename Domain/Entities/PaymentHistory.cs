using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class PaymentHistory : BaseEntity
    {
        public Guid PaymentId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty;

        public string? RawResponse { get; set; } // JSON from payment gateway

        public string? Note { get; set; }

        [ForeignKey("PaymentId")]
        public virtual Payment Payment { get; set; } = null!;
    }
}
