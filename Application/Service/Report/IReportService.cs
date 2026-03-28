using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs.Response.Report;

namespace Application.Service.Report
{
    public interface IReportService
    {
        // 1. Hàm lấy báo cáo tổng quan và danh sách theo ngày (Cho 3 card đầu & chart)
        Task<RevenueReportResponse> GetRevenueReportAsync(DateTime startDate, DateTime endDate);

        // 2. Hàm lấy chi tiết đơn hàng của 1 ngày cụ thể (Cho nút Xem chi tiết)
        Task<List<DayDetailOrderDto>> GetDayDetailOrdersAsync(DateTime date);

        // 3. Hàm xuất file báo cáo (Trả về mảng byte để Controller ép thành file tải về)
        Task<byte[]> ExportRevenueReportAsync(DateTime startDate, DateTime endDate);
    }
}