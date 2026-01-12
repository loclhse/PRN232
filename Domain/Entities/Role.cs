using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Domain.Enums;

namespace Domain.Entities
{
    public class Role : BaseEntity
    {
        
        [Required]
        public UserRole RoleName { get; set; }
        
        [MaxLength(250)]
        public string Description { get; set; } = string.Empty;

        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}
