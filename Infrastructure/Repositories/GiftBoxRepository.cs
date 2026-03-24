using Domain.Entities;
using Domain.IRepositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class GiftBoxRepository : GenericRepository<GiftBox>, IGiftBoxRepository
    {
        public GiftBoxRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<GiftBox?> GetByCodeAsync(string code)
        {
            return await dbSet
                .Include(g => g.Category)
                .Include(g => g.ComponentConfig)
                .Include(g => g.BoxComponents)
                    .ThenInclude(bc => bc.Product)
                .Include(g => g.Images)
                .FirstOrDefaultAsync(g => g.Code == code && !g.IsDeleted);
        }

        public async Task<bool> CodeExistsAsync(string code, Guid? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return await dbSet.AnyAsync(g => g.Code == code && g.Id != excludeId.Value && !g.IsDeleted);
            }
            return await dbSet.AnyAsync(g => g.Code == code && !g.IsDeleted);
        }

        public async Task<IEnumerable<GiftBox>> GetByCategoryAsync(Guid categoryId)
        {
            return await dbSet
                .Include(g => g.Category)
                .Include(g => g.ComponentConfig)
                .Include(g => g.Images)
                .Where(g => g.CategoryId == categoryId && !g.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<GiftBox>> GetActiveGiftBoxesAsync()
        {
            return await dbSet
                .Include(g => g.Category)
                .Include(g => g.ComponentConfig)
                .Include(g => g.Images)
                .Where(g => g.IsActive && !g.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<GiftBox>> GetGiftBoxesByUserIdAsync(Guid userId)
        {
            return await dbSet
                .Include(g => g.Category)
                .Include(g => g.ComponentConfig)
                .Include(g => g.BoxComponents)
                    .ThenInclude(bc => bc.Product)
                .Include(g => g.Images)
                .Where(g => g.UserId == userId && !g.IsDeleted)
                .ToListAsync();
        }
    }
}
