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

            // 3. Tìm Top Sản Phẩm bán chạy nhất (chỉ tính dòng có ProductId, bỏ qua GiftBox)
            var allOrderDetails = orders.SelectMany(o => o.OrderDetails).ToList();
            var productOrderDetails = allOrderDetails.Where(od => od.ProductId.HasValue).ToList();
            if (productOrderDetails.Any())
            {
                var topProductData = productOrderDetails
                    .GroupBy(od => od.ProductId!.Value)
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

        // 1. API BIỂU ĐỒ DOANH THU THEO THỜI GIAN (Chỉ tính đơn đã giao thành công)
        public async Task<List<SalesTrendDto>> GetSalesTrendAsync(DateTime startDate, DateTime endDate)
        {
            var orders = await _unitOfWork.OrderRepository.FindAsync(
                filter: o => o.CurrentStatus == OrderStatus.Delivered
                          && !o.IsDeleted
                          && o.CreatedAt >= startDate
                          && o.CreatedAt <= endDate
            );

            var result = orders
                .GroupBy(o => o.CreatedAt.Date) // Nhóm theo từng ngày
                .Select(g => new SalesTrendDto
                {
                    Date = g.Key.ToString("dd/MM/yyyy"), // Format ngày đẹp cho FE
                    Revenue = g.Sum(o => o.FinalAmount), // Tổng doanh thu trong ngày
                    OrderCount = g.Count()               // Số đơn trong ngày
                })
                .OrderBy(x => DateTime.ParseExact(x.Date, "dd/MM/yyyy", null)) // Sắp xếp từ ngày cũ tới mới
                .ToList();

            return result;
        }

        // 2. API TỈ LỆ TRẠNG THÁI ĐƠN HÀNG (Tính tất cả các đơn)
        public async Task<List<OrderStatusChartDto>> GetOrderStatusPieChartAsync(DateTime startDate, DateTime endDate)
        {
            var orders = await _unitOfWork.OrderRepository.FindAsync(
                filter: o => !o.IsDeleted
                          && o.CreatedAt >= startDate
                          && o.CreatedAt <= endDate
            );

            var result = orders
                .GroupBy(o => o.CurrentStatus) // Nhóm theo trạng thái
                .Select(g => new OrderStatusChartDto
                {
                    StatusName = g.Key.ToString(), // VD: "Pending", "Delivered"
                    Count = g.Count()              // Đếm số lượng
                })
                .ToList();

            return result;
        }

        // 3. API DANH SÁCH ĐƠN HÀNG MỚI NHẤT
        public async Task<List<RecentOrderDto>> GetRecentOrdersAsync(int limit)
        {
            // Lấy ra các đơn hàng, sắp xếp theo thời gian tạo mới nhất giảm dần
            var orders = await _unitOfWork.OrderRepository.FindAsync(
                filter: o => !o.IsDeleted,
                orderBy: q => q.OrderByDescending(o => o.CreatedAt)
            );

            var latestOrders = orders.Take(limit).ToList(); // Chỉ lấy đúng số lượng limit (VD: 5 đơn)
            var result = new List<RecentOrderDto>();

            var userRepo = _unitOfWork.Repository<Domain.Entities.User>();

            // Lặp để lấy thêm tên khách hàng
            foreach (var order in latestOrders)
            {
                var user = await userRepo.GetByIdAsync(order.UserId);

                result.Add(new RecentOrderDto
                {
                    OrderId = order.Id,
                    OrderNumber = order.OrderNumber,
                    CustomerName = user?.Username ?? user?.Email ?? "Khách hàng",
                    FinalAmount = order.FinalAmount,
                    Status = order.CurrentStatus.ToString(),
                    CreatedAt = order.CreatedAt
                });
            }

            return result;
        }


        // 4. API TOP SẢN PHẨM BÁN CHẠY NHẤT (Có thể bao gồm cả GiftBox, nhưng phải phân biệt được loại nào là Product, loại nào là GiftBox)
        public async Task<List<BestSellerItemDto>> GetBestSellersAsync(DateTime startDate, DateTime endDate, int limit = 5)
        {
            // 1. Lấy tất cả đơn hàng đã giao thành công
            var orders = await _unitOfWork.OrderRepository.FindAsync(
                filter: o => o.CurrentStatus == OrderStatus.Delivered
                          && !o.IsDeleted
                          && o.CreatedAt >= startDate
                          && o.CreatedAt <= endDate,
                includeProperties: "OrderDetails"
            );

            var result = new List<BestSellerItemDto>();
            if (!orders.Any()) return result;

            var allOrderDetails = orders.SelectMany(o => o.OrderDetails).ToList();

            // 2. Thống kê theo Product (Những dòng chi tiết có ProductId)
            var productSales = allOrderDetails
                .Where(od => od.ProductId.HasValue)
                .GroupBy(od => od.ProductId!.Value)
                .Select(g => new BestSellerItemDto
                {
                    ItemId = g.Key,
                    ItemType = "Product",
                    TotalSoldQuantity = g.Sum(od => od.Quantity),
                    TotalRevenue = g.Sum(od => od.Quantity * od.UnitPrice) //
                });

            // 3. Thống kê theo GiftBox của CỬA HÀNG (IsCustom == false)
            var giftBoxIdsInOrders = allOrderDetails
                .Where(od => od.GiftBoxId.HasValue)
                .Select(od => od.GiftBoxId!.Value)
                .Distinct()
                .ToList();

            // Truy vấn database để lọc ra các GiftBox của cửa hàng
            var storeGiftBoxes = await _unitOfWork.Repository<Domain.Entities.GiftBox>().FindAsync(
                filter: gb => giftBoxIdsInOrders.Contains(gb.Id) && gb.IsCustom == false //
            );
            var storeGiftBoxIds = storeGiftBoxes.Select(gb => gb.Id).ToHashSet();

            // Lấy doanh số của các GiftBox hợp lệ
            var giftBoxSales = allOrderDetails
                .Where(od => od.GiftBoxId.HasValue && storeGiftBoxIds.Contains(od.GiftBoxId.Value)) //
                .GroupBy(od => od.GiftBoxId!.Value)
                .Select(g => new BestSellerItemDto
                {
                    ItemId = g.Key,
                    ItemType = "GiftBox",
                    TotalSoldQuantity = g.Sum(od => od.Quantity),
                    TotalRevenue = g.Sum(od => od.Quantity * od.UnitPrice) //
                });

            // 4. Gộp cả 2 danh sách, Sắp xếp giảm dần theo số lượng bán và lấy limit
            var topItemsData = productSales.Concat(giftBoxSales)
                .OrderByDescending(x => x.TotalSoldQuantity)
                .Take(limit)
                .ToList();

            var productRepo = _unitOfWork.Repository<Domain.Entities.Product>();

            // 5. Gắn tên (ItemName) cho từng loại
            foreach (var item in topItemsData)
            {
                if (item.ItemType == "Product")
                {
                    var product = await productRepo.GetByIdAsync(item.ItemId);
                    item.ItemName = product?.Name ?? "Sản phẩm không xác định";
                }
                else if (item.ItemType == "GiftBox")
                {
                    var giftBox = storeGiftBoxes.FirstOrDefault(gb => gb.Id == item.ItemId);
                    item.ItemName = giftBox?.Name ?? "Giỏ quà không xác định";
                }

                result.Add(item);
            }

            return result;
        }
    }
}