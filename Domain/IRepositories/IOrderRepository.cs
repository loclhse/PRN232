using Domain.Entities;

namespace Domain.IRepositories
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        // Có thể thêm các method cụ thể cho Order nếu cần
    }
}
