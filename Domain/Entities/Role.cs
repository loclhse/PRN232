using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Role : BaseEntity
    {
        
        [Required]
        [MaxLength(50)]
        public string RoleName { get; set; } = string.Empty; // Admin, Staff, Customer, Guest
        
        [MaxLength(250)]
        public string Description { get; set; } = string.Empty;

        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}
