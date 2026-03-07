using Application.Service.Dashboard;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace PRN2322.Controllers
{
    [Route("api/dashboards")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        // ==========================================
        // 0. API Tổng quan (Summary)
        // ==========================================
        [HttpGet("summary")]
        public async Task<IActionResult> GetDashboardSummary([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (endDate < startDate)
                return BadRequest(new { success = false, message = "Ngày kết thúc không được nhỏ hơn ngày bắt đầu." });

            // [SỬA LỖI Ở ĐÂY]: Ép kiểu ngày giờ về chuẩn UTC cho PostgreSQL khỏi phàn nàn
            var adjustedStartDate = DateTime.SpecifyKind(startDate.Date, DateTimeKind.Utc);
            var adjustedEndDate = DateTime.SpecifyKind(endDate.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);

            var result = await _dashboardService.GetDashboardSummaryAsync(adjustedStartDate, adjustedEndDate);
            return Ok(new { success = true, data = result });
        }

        // ==========================================
        // 1. API Biểu đồ Doanh thu (Sales Trend)
        // ==========================================
        [HttpGet("sales-trend")]
        public async Task<IActionResult> GetSalesTrend([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (endDate < startDate)
                return BadRequest(new { success = false, message = "Ngày kết thúc không được nhỏ hơn ngày bắt đầu." });

            // Ép chuẩn UTC
            var adjustedStartDate = DateTime.SpecifyKind(startDate.Date, DateTimeKind.Utc);
            var adjustedEndDate = DateTime.SpecifyKind(endDate.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);

            var result = await _dashboardService.GetSalesTrendAsync(adjustedStartDate, adjustedEndDate);
            return Ok(new { success = true, data = result });
        }

        // ==========================================
        // 2. API Biểu đồ Tròn Trạng thái Đơn (Order Status)
        // ==========================================
        [HttpGet("order-status")]
        public async Task<IActionResult> GetOrderStatusPieChart([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (endDate < startDate)
                return BadRequest(new { success = false, message = "Ngày kết thúc không được nhỏ hơn ngày bắt đầu." });

            // Ép chuẩn UTC
            var adjustedStartDate = DateTime.SpecifyKind(startDate.Date, DateTimeKind.Utc);
            var adjustedEndDate = DateTime.SpecifyKind(endDate.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);

            var result = await _dashboardService.GetOrderStatusPieChartAsync(adjustedStartDate, adjustedEndDate);
            return Ok(new { success = true, data = result });
        }

        // ==========================================
        // 3. API Bảng Đơn hàng mới nhất (Recent Orders)
        // ==========================================
        [HttpGet("recent-orders")]
        public async Task<IActionResult> GetRecentOrders([FromQuery] int limit = 5)
        {
            if (limit <= 0) limit = 5;
            if (limit > 50) limit = 50;

            var result = await _dashboardService.GetRecentOrdersAsync(limit);
            return Ok(new { success = true, data = result });
        }
    }
}