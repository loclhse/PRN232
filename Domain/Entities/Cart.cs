using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Cart : BaseEntity
    {
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        public virtual ICollection<CartItem> Items { get; set; } = new List<CartItem>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
