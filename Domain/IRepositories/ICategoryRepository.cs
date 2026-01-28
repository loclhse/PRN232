using Domain.Entities;

namespace Domain.IRepositories
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        // Có thể thêm các method cụ thể cho Category nếu cần
    }
}
