using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain.Entities
{
    public class Order : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string OrderNumber { get; set; } = string.Empty;

        public Guid UserId { get; set; }
        public Guid? VoucherId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; } // Sum of items

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ShippingFee { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal FinalAmount { get; set; } // Total - Discount + Shipping

        public OrderStatus CurrentStatus { get; set; } = OrderStatus.Pending;

        public string? Note { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("VoucherId")]
        public virtual Voucher? Voucher { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public virtual ICollection<OrderHistory> OrderHistories { get; set; } = new List<OrderHistory>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
