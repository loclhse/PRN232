using Domain.Entities;

namespace Domain.IRepositories
{
    public interface ICartItemRepository : IGenericRepository<CartItem>
    {
        // L?y CartItem theo CartId và ProductId
        Task<CartItem?> GetByCartAndProductAsync(Guid cartId, Guid productId);

        // L?y CartItem theo CartId và GiftBoxId
        Task<CartItem?> GetByCartAndGiftBoxAsync(Guid cartId, Guid giftBoxId);

        // L?y t?t c? Items c?a m?t Cart
        Task<IEnumerable<CartItem>> GetItemsByCartIdAsync(Guid cartId);

        // Xóa t?t c? Items c?a m?t Cart
        Task ClearCartItemsAsync(Guid cartId);
    }
}
