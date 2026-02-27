using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Request.Voucher
{
    public class CreateVoucherRequest
    {
        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty; // Mã voucher (VD: TET2025)

        [MaxLength(250)]
        public string Description { get; set; } = string.Empty;
        public bool IsPercentage { get; set; } // True: Giảm theo %, False: Giảm tiền mặt

        public decimal Value { get; set; }
        public decimal MinOrderValue { get; set; }
        public decimal? MaxDiscountAmount { get; set; }// Chỉ áp dụng nếu IsPercentage = true, giới hạn số tiền giảm tối đa
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int UsageLimit { get; set; }
    }
}