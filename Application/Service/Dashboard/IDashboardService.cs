using System;
using System.Threading.Tasks;
using Application.DTOs.Response.Dashboard;

namespace Application.Service.Dashboard
{
    public interface IDashboardService
    {
        Task<DashboardSummaryResponse> GetDashboardSummaryAsync(DateTime startDate, DateTime endDate);
    }
}