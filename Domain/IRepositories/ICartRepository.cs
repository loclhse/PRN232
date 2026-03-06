using Domain.Entities;

namespace Domain.IRepositories
{
    public interface ICartRepository : IGenericRepository<Cart>
    {
        // Lấy Cart của User kèm theo Items và thông tin Product/GiftBox
        Task<Cart?> GetCartByUserIdAsync(Guid userId);

        // Lấy Cart active (chưa checkout) của User
        Task<Cart?> GetActiveCartByUserIdAsync(Guid userId);

        // Lấy Cart theo ID kèm đầy đủ thông tin
        Task<Cart?> GetCartWithItemsAsync(Guid cartId);
    }
}
