using Domain.IRepositories;

namespace Domain.IUnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        // Specific repositories
        IUserRepository UserRepository { get; }
        IProductRepository ProductRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IOrderRepository OrderRepository { get; }
        IRoleRepository RoleRepository { get; }
        
        // Generic repository method (for other entities if needed)
        IGenericRepository<T> Repository<T>() where T : class;
        
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
