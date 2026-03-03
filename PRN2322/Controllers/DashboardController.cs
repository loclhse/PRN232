using Application.Service.Dashboard;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("summary")]
        public async Task<IActionResult> GetDashboardSummary([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (endDate < startDate)
            {
                return BadRequest(new { success = false, message = "Ngày kết thúc không được nhỏ hơn ngày bắt đầu." });
            }

            // Mẹo cực kỳ quan trọng: Ép endDate đến 23:59:59 của ngày đó để lấy trọn vẹn đơn hàng trong ngày
            var adjustedEndDate = endDate.Date.AddDays(1).AddTicks(-1);
            var adjustedStartDate = startDate.Date;

            var result = await _dashboardService.GetDashboardSummaryAsync(adjustedStartDate, adjustedEndDate);

            return Ok(new
            {
                success = true,
                message = "Lấy dữ liệu thống kê thành công",
                data = result
            });
        }

        // ==========================================
        // 1. API Biểu đồ Doanh thu (Sales Trend - Bar/Line Chart)
        // ==========================================
        [HttpGet("sales-trend")]
        public async Task<IActionResult> GetSalesTrend([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (endDate < startDate)
                return BadRequest(new { success = false, message = "Ngày kết thúc không được nhỏ hơn ngày bắt đầu." });

            var result = await _dashboardService.GetSalesTrendAsync(startDate.Date, endDate.Date.AddDays(1).AddTicks(-1));
            return Ok(new { success = true, data = result });
        }

        // ==========================================
        // 2. API Biểu đồ Tròn Trạng thái Đơn (Order Status - Pie Chart)
        // ==========================================
        [HttpGet("order-status")]
        public async Task<IActionResult> GetOrderStatusPieChart([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (endDate < startDate)
                return BadRequest(new { success = false, message = "Ngày kết thúc không được nhỏ hơn ngày bắt đầu." });

            var result = await _dashboardService.GetOrderStatusPieChartAsync(startDate.Date, endDate.Date.AddDays(1).AddTicks(-1));
            return Ok(new { success = true, data = result });
        }

        // ==========================================
        // 3. API Bảng Đơn hàng mới nhất (Recent Orders - Table)
        // ==========================================
        [HttpGet("recent-orders")]
        public async Task<IActionResult> GetRecentOrders([FromQuery] int limit = 5)
        {
            // Bảo mật nhẹ: Không cho FE truyền số âm hoặc số quá lớn gây sập server
            if (limit <= 0) limit = 5;
            if (limit > 50) limit = 50;

            var result = await _dashboardService.GetRecentOrdersAsync(limit);
            return Ok(new { success = true, data = result });
        }
    }
}