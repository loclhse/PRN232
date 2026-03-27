using System;
using System.Threading.Tasks;
using Application.DTOs.Response.Dashboard;

namespace Application.Service.Dashboard
{
    public interface IDashboardService
    {
        Task<DashboardSummaryResponse> GetDashboardSummaryAsync(DateTime startDate, DateTime endDate);
        Task<List<SalesTrendDto>> GetSalesTrendAsync(DateTime startDate, DateTime endDate);
        Task<List<OrderStatusChartDto>> GetOrderStatusPieChartAsync(DateTime startDate, DateTime endDate);
        Task<List<RecentOrderDto>> GetRecentOrdersAsync(int limit);
        Task<List<BestSellerItemDto>> GetBestSellersAsync(DateTime startDate, DateTime endDate, int limit = 5);
    }
}