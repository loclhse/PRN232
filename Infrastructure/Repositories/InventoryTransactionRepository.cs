using Domain.Entities;
using Domain.IRepositories;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
    public class InventoryTransactionRepository : GenericRepository<InventoryTransaction>, IInventoryTransactionRepository
    {
        public InventoryTransactionRepository(AppDbContext context) : base(context)
        {
        }

        // Có th? override ho?c thêm các method c? th? cho InventoryTransaction ? ?ây
    }
}
