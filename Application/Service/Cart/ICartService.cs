using Application.DTOs.Request.Cart;
using Application.DTOs.Response.Cart;
using Application.DTOs.Response.Order;

namespace Application.Service.Cart
{
    public interface ICartService
    {
        // Lấy giỏ hàng của user hiện tại
        Task<CartResponse?> GetCartByUserIdAsync(Guid userId);

        // Lấy hoặc tạo mới giỏ hàng cho user
        Task<CartResponse> GetOrCreateCartAsync(Guid userId);

        // Thêm sản phẩm/GiftBox vào giỏ hàng
        Task<CartResponse> AddToCartAsync(Guid userId, AddToCartRequest request);

        // Cập nhật số lượng item trong giỏ
        Task<CartResponse?> UpdateCartItemAsync(Guid userId, Guid cartItemId, UpdateCartItemRequest request);

        // Xóa một item khỏi giỏ hàng
        Task<bool> RemoveCartItemAsync(Guid userId, Guid cartItemId);

        // Xóa nhiều items khỏi giỏ hàng
        Task<bool> RemoveCartItemsAsync(Guid userId, List<Guid> cartItemIds);

        // Xóa toàn bộ giỏ hàng
        Task<bool> ClearCartAsync(Guid userId);

        // Checkout giỏ hàng - tạo Order từ Cart
        Task<OrderResponse> CheckoutAsync(Guid userId, CheckoutRequest request);

        // Lấy số lượng items trong giỏ (để hiển thị badge)
        Task<int> GetCartItemCountAsync(Guid userId);
    }
}
