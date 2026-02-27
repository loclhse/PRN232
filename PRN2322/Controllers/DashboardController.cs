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
    }
}