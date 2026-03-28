using System.Collections.Generic;

namespace Application.DTOs.Response.Report
{
    public class RevenueReportResponse
    {
        // Số liệu kỳ hiện tại (theo khoảng thời gian FE truyền)
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int DeliveredOrders { get; set; }
        public int CancelledOrders { get; set; }
        public int TotalProductsSold { get; set; }

        // Số liệu kỳ trước (để so sánh)
        public decimal PreviousRevenue { get; set; }
        public int PreviousOrders { get; set; }
        public int PreviousProductsSold { get; set; }

        // Phần trăm tăng trưởng (Growth)
        public double RevenueGrowthPercent { get; set; }
        public double OrderGrowthPercent { get; set; }
        public double ProductGrowthPercent { get; set; }

        // Danh sách báo cáo chi tiết từng ngày
        public List<DailyReportDto> DailyReports { get; set; } = new List<DailyReportDto>();
    }
}