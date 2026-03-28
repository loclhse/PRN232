using System;

namespace Application.DTOs.Response.Report
{
    public class DailyReportDto
    {
        public string Date { get; set; } = string.Empty; // Format: YYYY-MM-DD theo chuẩn FE cần
        public decimal Revenue { get; set; }
        public int TotalOrders { get; set; }
        public int DeliveredOrders { get; set; }
        public int CancelledOrders { get; set; }
        public int ProductsSold { get; set; }
    }
}