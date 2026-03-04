using Domain.Entities;

namespace Domain.IRepositories
{
    public interface IGiftBoxComponentConfigRepository : IGenericRepository<GiftBoxComponentConfig>
    {
        Task<IEnumerable<GiftBoxComponentConfig>> GetByCategoryAsync(string category);
        Task<IEnumerable<GiftBoxComponentConfig>> GetActiveConfigsAsync();
        Task<GiftBoxComponentConfig?> GetByIdWithGiftBoxAsync(Guid id);
    }
}
