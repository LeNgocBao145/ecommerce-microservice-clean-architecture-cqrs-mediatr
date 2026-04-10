using OrderService.Domain.Entities;
using System.Linq.Expressions;

namespace OrderService.Domain.Interfaces
{
    public interface ICartRepository
    {
        Task CreateCartAsync(Cart entity);
        Task CreateCartItemAsync(CartItem entity);
        Task<Cart?> FindByIdAsync(Guid id);
        Task<IEnumerable<Cart>> GetAllAsync();
        Task<Cart?> GetCartByAsync(Expression<Func<Cart, bool>> predicate);
        void UpdateAsync(Cart entity);
        Task DeleteAsync(Guid id);
        Task<CartItem?> GetCartItemByAsync(Expression<Func<CartItem, bool>> predicate);
        Task<IEnumerable<CartItem>> GetCartItemsByAsync(Expression<Func<CartItem, bool>> predicate);
        void UpdateAsync(CartItem cartItem);
    }
}
