using Domain.Entities;
using Domain.IRepositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CartRepository : GenericRepository<Cart>, ICartRepository
    {
        public CartRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Cart?> GetCartByUserIdAsync(Guid userId)
        {
            return await dbSet
                .Include(c => c.User)
                .Include(c => c.Items.Where(i => !i.IsDeleted))
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p!.Images)
                .Include(c => c.Items.Where(i => !i.IsDeleted))
                    .ThenInclude(i => i.GiftBox)
                        .ThenInclude(g => g!.Images)
                .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsDeleted);
        }

        public async Task<Cart?> GetActiveCartByUserIdAsync(Guid userId)
        {
            // Cart active là Cart ch?a ???c checkout (không có Order nào liên k?t ho?c Order ch?a hoàn thành)
            return await dbSet
                .Include(c => c.User)
                .Include(c => c.Items.Where(i => !i.IsDeleted))
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p!.Images)
                .Include(c => c.Items.Where(i => !i.IsDeleted))
                    .ThenInclude(i => i.GiftBox)
                        .ThenInclude(g => g!.Images)
                .Where(c => c.UserId == userId && !c.IsDeleted)
                .OrderByDescending(c => c.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<Cart?> GetCartWithItemsAsync(Guid cartId)
        {
            return await dbSet
                .Include(c => c.User)
                .Include(c => c.Items.Where(i => !i.IsDeleted))
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p!.Category)
                .Include(c => c.Items.Where(i => !i.IsDeleted))
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p!.Images)
                .Include(c => c.Items.Where(i => !i.IsDeleted))
                    .ThenInclude(i => i.GiftBox)
                        .ThenInclude(g => g!.Category)
                .Include(c => c.Items.Where(i => !i.IsDeleted))
                    .ThenInclude(i => i.GiftBox)
                        .ThenInclude(g => g!.Images)
                .FirstOrDefaultAsync(c => c.Id == cartId && !c.IsDeleted);
        }
    }
}
