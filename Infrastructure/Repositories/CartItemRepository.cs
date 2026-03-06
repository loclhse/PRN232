using Domain.Entities;
using Domain.IRepositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CartItemRepository : GenericRepository<CartItem>, ICartItemRepository
    {
        public CartItemRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<CartItem?> GetByCartAndProductAsync(Guid cartId, Guid productId)
        {
            return await dbSet
                .Include(ci => ci.Product)
                    .ThenInclude(p => p!.Images)
                .FirstOrDefaultAsync(ci => ci.CartId == cartId 
                    && ci.ProductId == productId 
                    && !ci.IsDeleted);
        }

        public async Task<CartItem?> GetByCartAndGiftBoxAsync(Guid cartId, Guid giftBoxId)
        {
            return await dbSet
                .Include(ci => ci.GiftBox)
                    .ThenInclude(g => g!.Images)
                .FirstOrDefaultAsync(ci => ci.CartId == cartId 
                    && ci.GiftBoxId == giftBoxId 
                    && !ci.IsDeleted);
        }

        public async Task<IEnumerable<CartItem>> GetItemsByCartIdAsync(Guid cartId)
        {
            return await dbSet
                .Include(ci => ci.Product)
                    .ThenInclude(p => p!.Images)
                .Include(ci => ci.GiftBox)
                    .ThenInclude(g => g!.Images)
                .Where(ci => ci.CartId == cartId && !ci.IsDeleted)
                .ToListAsync();
        }

        public async Task ClearCartItemsAsync(Guid cartId)
        {
            var items = await dbSet
                .Where(ci => ci.CartId == cartId && !ci.IsDeleted)
                .ToListAsync();

            foreach (var item in items)
            {
                item.IsDeleted = true;
                item.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}
