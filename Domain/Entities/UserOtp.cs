using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class UserOtp : BaseEntity
    {
        public int UserId { get; set; }
        
        [Required]
        [MaxLength(10)]
        public string OtpCode { get; set; } = string.Empty;

        public DateTime ExpiryTime { get; set; }
        
        public bool IsUsed { get; set; } = false;

        [MaxLength(50)]
        public string OtpType { get; set; } = "Login"; // Login, ResetPassword, etc.

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}
