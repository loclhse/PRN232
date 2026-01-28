using Domain.Entities;

namespace Domain.IRepositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        // Có thể thêm các method cụ thể cho User nếu cần
    }
}
