using System;

namespace Application.DTOs.Response.Dashboard
{
    public class DashboardSummaryResponse
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public TopProductDto? TopProduct { get; set; }
        public TopCustomerDto? TopCustomer { get; set; }
    }

    public class TopProductDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int TotalSoldQuantity { get; set; }
        public decimal RevenueFromProduct { get; set; }
    }

    public class TopCustomerDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public decimal TotalSpent { get; set; }
        public int TotalOrdersPlaced { get; set; }
    }
}