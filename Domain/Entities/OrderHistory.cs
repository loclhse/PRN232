using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain.Entities
{
    public class OrderHistory : BaseEntity
    {
        public int OrderId { get; set; }

        public OrderStatus Status { get; set; }

        public string? Note { get; set; }

        [MaxLength(100)]
        public string? ChangedBy { get; set; } // Username

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; } = null!;
    }
}
