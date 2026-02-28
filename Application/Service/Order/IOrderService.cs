using Application.DTOs.Request.Order;
using Application.DTOs.Response.Order;
using Domain.Enums;

namespace Application.Service.Order
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderResponse>> GetAllOrdersAsync();
        Task<OrderResponse?> GetOrderByIdAsync(Guid id);
        Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request);
        Task<OrderResponse?> UpdateOrderStatusAsync(Guid id, OrderStatus newStatus);
        Task<bool> DeleteOrderAsync(Guid id);
    }
}