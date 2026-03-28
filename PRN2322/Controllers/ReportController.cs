using Application.Service.Report;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace PRN2322.Controllers
{
    [Route("api/reports")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        // ==========================================
        // 1. API BÁO CÁO TỔNG QUAN & BIỂU ĐỒ
        // GET: /api/reports/revenue?startDate=...&endDate=...
        // ==========================================
        [HttpGet("revenue")]
        public async Task<IActionResult> GetRevenueReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (endDate < startDate)
                return BadRequest(new { success = false, message = "Ngày kết thúc không được nhỏ hơn ngày bắt đầu." });

            var result = await _reportService.GetRevenueReportAsync(startDate, endDate);
            return Ok(new { success = true, data = result });
        }

        // ==========================================
        // 2. API CHI TIẾT ĐƠN HÀNG TRONG 1 NGÀY
        // GET: /api/reports/revenue/day-details?date=YYYY-MM-DD
        // ==========================================
        [HttpGet("revenue/day-details")]
        public async Task<IActionResult> GetDayDetailOrders([FromQuery] DateTime date)
        {
            var result = await _reportService.GetDayDetailOrdersAsync(date);
            return Ok(new { success = true, data = result });
        }

        // ==========================================
        // 3. API XUẤT FILE BÁO CÁO (CSV)
        // GET: /api/reports/revenue/export?startDate=...&endDate=...
        // ==========================================
        [HttpGet("revenue/export")]
        public async Task<IActionResult> ExportRevenueReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (endDate < startDate)
                return BadRequest(new { success = false, message = "Ngày kết thúc không được nhỏ hơn ngày bắt đầu." });

            // Lấy cục mảng byte[] (nội dung file) từ Service
            var fileBytes = await _reportService.ExportRevenueReportAsync(startDate, endDate);

            // Đặt tên file tự động theo khoảng thời gian báo cáo cho ngầu
            string fileName = $"Bao_Cao_Doanh_Thu_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.csv";

            // Dùng hàm File() của ControllerBase để ép kiểu trả về dạng File Tải xuống
            return File(fileBytes, "text/csv", fileName);
        }
    }
}