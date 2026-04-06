using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace OrderService.Infrastructure.Repositories
{
    public class OrderRepository(AppDbContext context) : IOrderRepository
    {
        private readonly AppDbContext _context = context;

        public async Task CreateAsync(Order entity)
        {
            await _context.Orders.AddAsync(entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            return;
        }

        public async Task<Order?> FindByIdAsync(Guid id)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderByAsync(Expression<Func<Order, bool>> predicate)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<Order>> GetOrdersByAsync(Expression<Func<Order, bool>> predicate)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .Where(predicate)
                .ToListAsync();
        }

        public void UpdateAsync(Order entity)
        {
            _context.Orders.Update(entity);
        }
    }
}
