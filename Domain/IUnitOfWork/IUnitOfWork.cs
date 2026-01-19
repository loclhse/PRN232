using Domain.IRepositories;

namespace Domain.IUnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        // Add specific repositories here as needed
        IProductRepository productRepository { get; }
        ICategoryRepository categoryRepository { get; }
        
        IGenericRepository<T> Repository<T>() where T : class;
        
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
