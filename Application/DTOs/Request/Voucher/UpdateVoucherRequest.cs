namespace Application.DTOs.Request.Voucher
{
    public class UpdateVoucherRequest
    {
        public string Description { get; set; } = string.Empty;

        public bool IsPercentage { get; set; } // Dùng bool

        public decimal Value { get; set; }
        public decimal MinOrderValue { get; set; }
        public decimal? MaxDiscountAmount { get; set; }// Chỉ áp dụng nếu IsPercentage = true, giới hạn số tiền giảm tối đa
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int UsageLimit { get; set; }
        public bool IsActive { get; set; }
    }
}