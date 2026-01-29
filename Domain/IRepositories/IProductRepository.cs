using Domain.Entities;

namespace Domain.IRepositories
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        // Có thể thêm các method cụ thể cho Product nếu cần
    }
}
