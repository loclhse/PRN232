using Domain.Entities;

namespace Domain.IRepositories
{
    public interface IGiftBoxRepository : IGenericRepository<GiftBox>
    {
        Task<GiftBox?> GetByCodeAsync(string code);
        Task<bool> CodeExistsAsync(string code, Guid? excludeId = null);
        Task<IEnumerable<GiftBox>> GetByCategoryAsync(Guid categoryId);
        Task<IEnumerable<GiftBox>> GetActiveGiftBoxesAsync();
    }
}
