using OrderService.Domain.Entities;
using System.Linq.Expressions;

namespace OrderService.Domain.Interfaces
{
    public interface IOrderRepository
    {
        Task CreateAsync(Order entity);

        Task DeleteAsync(Guid id);

        Task<Order?> FindByIdAsync(Guid id);

        Task<IEnumerable<Order>> GetAllAsync();

        Task<Order?> GetOrderByAsync(Expression<Func<Order, bool>> predicate);

        Task<IEnumerable<Order>> GetOrdersByAsync(Expression<Func<Order, bool>> predicate);

        void UpdateAsync(Order entity);
    }
}
