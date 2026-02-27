using Application.DTOs.Response.Dashboard;
using Domain.Entities;
using Domain.Enums;
using Domain.IUnitOfWork;

namespace Application.Service.Dashboard
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DashboardSummaryResponse> GetDashboardSummaryAsync(DateTime startDate, DateTime endDate)
        {
            // 1. Lấy tất cả đơn hàng HỢP LỆ (Đã giao thành công) trong khoảng thời gian
            var orders = await _unitOfWork.OrderRepository.FindAsync(
                filter: o => o.CurrentStatus == OrderStatus.Delivered
                          && !o.IsDeleted
                          && o.CreatedAt >= startDate
                          && o.CreatedAt <= endDate,
                includeProperties: "OrderDetails" // Lấy kèm chi tiết để tính top sản phẩm
            );

            var response = new DashboardSummaryResponse();

            if (!orders.Any()) return response; // Trả về 0 nếu không có đơn nào

            // 2. Tính Doanh thu và Số lượng đơn
            response.TotalRevenue = orders.Sum(o => o.FinalAmount);
            response.TotalOrders = orders.Count();

            // 3. Tìm Top Sản Phẩm bán chạy nhất
            var allOrderDetails = orders.SelectMany(o => o.OrderDetails).ToList();
            if (allOrderDetails.Any())
            {
                var topProductData = allOrderDetails
                    .GroupBy(od => od.ProductId)
                    .Select(g => new
                    {
                        ProductId = g.Key,
                        TotalQuantity = g.Sum(od => od.Quantity),
                        TotalRevenue = g.Sum(od => od.Quantity * od.UnitPrice)
                    })
                    .OrderByDescending(x => x.TotalQuantity) // Xếp giảm dần theo số lượng
                    .FirstOrDefault(); // Lấy thằng đứng đầu

                if (topProductData != null)
                {
                    var product = await _unitOfWork.Repository<Domain.Entities.Product>().GetByIdAsync(topProductData.ProductId);
                    response.TopProduct = new TopProductDto
                    {
                        ProductId = topProductData.ProductId,
                        ProductName = product?.Name ?? "Sản phẩm không xác định",
                        TotalSoldQuantity = topProductData.TotalQuantity,
                        RevenueFromProduct = topProductData.TotalRevenue
                    };
                }
            }

            // 4. Tìm Top Khách Hàng (Chi nhiều tiền nhất)
            var topCustomerData = orders
                .GroupBy(o => o.UserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    TotalSpent = g.Sum(o => o.FinalAmount),
                    TotalOrders = g.Count()
                })
                .OrderByDescending(x => x.TotalSpent) // Xếp giảm dần theo tổng tiền đã chi
                .FirstOrDefault();

            if (topCustomerData != null)
            {
                var user = await _unitOfWork.Repository<Domain.Entities.User>().GetByIdAsync(topCustomerData.UserId);
                response.TopCustomer = new TopCustomerDto
                {
                    UserId = topCustomerData.UserId,
                    UserName = user?.Username ?? user?.Email ?? "Khách hàng ẩn danh", // Hoặc user.FullName nếu DB bạn có
                    TotalSpent = topCustomerData.TotalSpent,
                    TotalOrdersPlaced = topCustomerData.TotalOrders
                };
            }

            return response;
        }
    }
}