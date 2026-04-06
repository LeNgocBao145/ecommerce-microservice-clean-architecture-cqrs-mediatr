using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace OrderService.Infrastructure.Repositories
{
    public class CartRepository(AppDbContext context) : ICartRepository
    {
        private readonly AppDbContext _context = context;

        public async Task CreateAsync(Cart entity)
        {
            await _context.Carts.AddAsync(entity);
        }

        public async void DeleteAsync(Guid id)
        {
            var cart = await _context.Carts.FindAsync(id);

            if (cart != null)
            {
                _context.Carts.Remove(cart);
            }
        }

        public async Task<Cart?> FindByIdAsync(Guid id)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Cart>> GetAllAsync()
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                .ToListAsync();
        }

        public async Task<Cart?> GetCartByAsync(Expression<Func<Cart, bool>> predicate)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(predicate);
        }

        public async Task<CartItem?> GetCartItemByAsync(Expression<Func<CartItem, bool>> predicate)
        {
            return await _context.CartItems
                .Where(predicate).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<CartItem>> GetCartItemsByAsync(Expression<Func<CartItem, bool>> predicate)
        {
            return await _context.CartItems
                .Where(predicate).ToListAsync();
        }

        public void UpdateAsync(Cart entity)
        {
            _context.Carts.Update(entity);
        }

        public void UpdateAsync(CartItem entity)
        {
            _context.CartItems.Update(entity);
        }
    }
}
