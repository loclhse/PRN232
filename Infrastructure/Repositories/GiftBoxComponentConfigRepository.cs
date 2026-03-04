using Domain.Entities;
using Domain.IRepositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class GiftBoxComponentConfigRepository : GenericRepository<GiftBoxComponentConfig>, IGiftBoxComponentConfigRepository
    {
        public GiftBoxComponentConfigRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<GiftBoxComponentConfig>> GetByCategoryAsync(string category)
        {
            return await dbSet
                .Include(c => c.GiftBox)
                .Where(c => c.Category == category && !c.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<GiftBoxComponentConfig>> GetActiveConfigsAsync()
        {
            return await dbSet
                .Include(c => c.GiftBox)
                .Where(c => c.IsActive && !c.IsDeleted)
                .ToListAsync();
        }

        public async Task<GiftBoxComponentConfig?> GetByIdWithGiftBoxAsync(Guid id)
        {
            return await dbSet
                .Include(c => c.GiftBox)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }
    }
}
