using System;

namespace Application.DTOs.Response.Dashboard
{
    // 1. DTO cho Biểu đồ Doanh thu (Sales Trend)
    public class SalesTrendDto
    {
        public string Date { get; set; } = string.Empty; // Ngày hoặc Tháng (Ví dụ: "25/02/2026")
        public decimal Revenue { get; set; }             // Doanh thu trong ngày đó
        public int OrderCount { get; set; }              // Số lượng đơn hàng trong ngày đó
    }

    // 2. DTO cho Biểu đồ Tròn Trạng thái (Order Status Pie Chart)
    public class OrderStatusChartDto
    {
        public string StatusName { get; set; } = string.Empty; // Tên trạng thái (Pending, Delivered...)
        public int Count { get; set; }                         // Số lượng đơn
    }

    // 3. DTO cho Bảng Đơn hàng mới nhất (Recent Orders)
    public class RecentOrderDto
    {
        public Guid OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty; // Lấy từ UserName hoặc Email
        public decimal FinalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}