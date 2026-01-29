using Domain.Entities;
using Domain.IRepositories;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context)
        {
        }

        // Có thể override hoặc thêm các method cụ thể cho Product ở đây
    }
}
