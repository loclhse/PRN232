using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Response.Report;
using Domain.Enums;
using Domain.IUnitOfWork;

namespace Application.Service.Report
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ==========================================
        // 1. API BÁO CÁO TỔNG QUAN & BIỂU ĐỒ (Card + Chart)
        // ==========================================
        public async Task<RevenueReportResponse> GetRevenueReportAsync(DateTime startDate, DateTime endDate)
        {
            // Ép về UTC chuẩn để query DB
            var startUtc = DateTime.SpecifyKind(startDate.Date, DateTimeKind.Utc);
            var endUtc = DateTime.SpecifyKind(endDate.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);

            // 1. Lấy dữ liệu kỳ này
            var currentOrders = await _unitOfWork.OrderRepository.FindAsync(
                filter: o => !o.IsDeleted && o.CreatedAt >= startUtc && o.CreatedAt <= endUtc,
                includeProperties: "OrderDetails"
            );

            // 2. Tính toán khoảng thời gian kỳ trước (Previous Period) để so sánh
            var duration = endUtc - startUtc;
            var prevEndUtc = startUtc.AddTicks(-1);
            var prevStartUtc = prevEndUtc.Subtract(duration);

            var previousOrders = await _unitOfWork.OrderRepository.FindAsync(
                filter: o => !o.IsDeleted && o.CreatedAt >= prevStartUtc && o.CreatedAt <= prevEndUtc,
                includeProperties: "OrderDetails"
            );

            var response = new RevenueReportResponse();

            // 3. THỐNG KÊ KỲ NÀY
            response.TotalOrders = currentOrders.Count();
            response.DeliveredOrders = currentOrders.Count(o => o.CurrentStatus == OrderStatus.Delivered);
            response.CancelledOrders = currentOrders.Count(o => o.CurrentStatus == OrderStatus.Cancelled);
            response.TotalRevenue = currentOrders.Where(o => o.CurrentStatus == OrderStatus.Delivered).Sum(o => o.FinalAmount);
            response.TotalProductsSold = currentOrders
                .Where(o => o.CurrentStatus == OrderStatus.Delivered)
                .SelectMany(o => o.OrderDetails)
                .Sum(od => od.Quantity);

            // 4. THỐNG KÊ KỲ TRƯỚC
            response.PreviousOrders = previousOrders.Count();
            response.PreviousRevenue = previousOrders.Where(o => o.CurrentStatus == OrderStatus.Delivered).Sum(o => o.FinalAmount);
            response.PreviousProductsSold = previousOrders
                .Where(o => o.CurrentStatus == OrderStatus.Delivered)
                .SelectMany(o => o.OrderDetails)
                .Sum(od => od.Quantity);

            // 5. TÍNH PHẦN TRĂM TĂNG TRƯỞNG (Growth %)
            response.RevenueGrowthPercent = CalculateGrowth(response.PreviousRevenue, response.TotalRevenue);
            response.OrderGrowthPercent = CalculateGrowth(response.PreviousOrders, response.TotalOrders);
            response.ProductGrowthPercent = CalculateGrowth(response.PreviousProductsSold, response.TotalProductsSold);

            // 6. NHÓM DỮ LIỆU THEO TỪNG NGÀY CHO BIỂU ĐỒ (Daily Reports)
            var dailyGroups = currentOrders
                .GroupBy(o => o.CreatedAt.Date)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Lặp từ ngày bắt đầu đến ngày kết thúc để đảm bảo những ngày không có đơn vẫn hiển thị số 0
            for (var date = startUtc.Date; date <= endUtc.Date; date = date.AddDays(1))
            {
                var ordersInDay = dailyGroups.ContainsKey(date) ? dailyGroups[date] : new List<Domain.Entities.Order>();

                response.DailyReports.Add(new DailyReportDto
                {
                    Date = date.ToString("yyyy-MM-dd"), // Format chuẩn cho FE
                    TotalOrders = ordersInDay.Count,
                    DeliveredOrders = ordersInDay.Count(o => o.CurrentStatus == OrderStatus.Delivered),
                    CancelledOrders = ordersInDay.Count(o => o.CurrentStatus == OrderStatus.Cancelled),
                    Revenue = ordersInDay.Where(o => o.CurrentStatus == OrderStatus.Delivered).Sum(o => o.FinalAmount),
                    ProductsSold = ordersInDay.Where(o => o.CurrentStatus == OrderStatus.Delivered)
                                              .SelectMany(o => o.OrderDetails).Sum(od => od.Quantity)
                });
            }

            return response;
        }

        // ==========================================
        // 2. API CHI TIẾT ĐƠN HÀNG 1 NGÀY (Nut Xem chi tiết)
        // ==========================================
        public async Task<List<DayDetailOrderDto>> GetDayDetailOrdersAsync(DateTime date)
        {
            var startUtc = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
            var endUtc = startUtc.AddDays(1).AddTicks(-1);

            var orders = await _unitOfWork.OrderRepository.FindAsync(
                filter: o => !o.IsDeleted && o.CreatedAt >= startUtc && o.CreatedAt <= endUtc,
                orderBy: q => q.OrderByDescending(o => o.CreatedAt)
            );

            var userRepo = _unitOfWork.Repository<Domain.Entities.User>();
            var result = new List<DayDetailOrderDto>();

            foreach (var order in orders)
            {
                var user = await userRepo.GetByIdAsync(order.UserId);
                result.Add(new DayDetailOrderDto
                {
                    OrderId = order.Id,
                    OrderNumber = order.OrderNumber,
                    CustomerName = user?.Username ?? user?.Email ?? "Khách ẩn danh",
                    FinalAmount = order.FinalAmount,
                    Status = order.CurrentStatus.ToString(),
                    CreatedAt = order.CreatedAt
                });
            }

            return result;
        }

        // ==========================================
        // 3. API XUẤT FILE CSV
        // ==========================================
        public async Task<byte[]> ExportRevenueReportAsync(DateTime startDate, DateTime endDate)
        {
            // Gọi lại hàm ở trên để tái sử dụng logic tính toán
            var reportData = await GetRevenueReportAsync(startDate, endDate);

            var csvBuilder = new StringBuilder();

            // Thêm BOM (Byte Order Mark) để Excel mở file CSV tiếng Việt không bị lỗi font
            csvBuilder.AppendLine("Ngày,Doanh Thu (VNĐ),Tổng số đơn,Đơn thành công,Đơn hủy,Số SP bán ra");

            foreach (var day in reportData.DailyReports)
            {
                csvBuilder.AppendLine($"{day.Date},{day.Revenue},{day.TotalOrders},{day.DeliveredOrders},{day.CancelledOrders},{day.ProductsSold}");
            }

            // Dòng tổng kết ở cuối file
            csvBuilder.AppendLine($"TỔNG CỘNG,{reportData.TotalRevenue},{reportData.TotalOrders},{reportData.DeliveredOrders},{reportData.CancelledOrders},{reportData.TotalProductsSold}");

            // Chuyển string thành byte array chuẩn UTF-8 (CÓ BOM)
            return new UTF8Encoding(true).GetBytes(csvBuilder.ToString());
        }

        // --- Hàm phụ trợ để tính % ---
        private double CalculateGrowth(decimal previous, decimal current)
        {
            if (previous == 0) return current > 0 ? 100 : 0; // Tránh lỗi chia cho 0
            return (double)((current - previous) / previous * 100);
        }
    }
}