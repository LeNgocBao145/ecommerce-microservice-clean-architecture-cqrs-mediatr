using PromotionService.Domain.Entities;
using System.Linq.Expressions;

namespace PromotionService.Domain.Interfaces
{
    public interface ICouponRepository
    {
        Task<Coupon> CreateAsync(Coupon entity);
        Task<Coupon> UpdateAsync(Coupon entity);
        Task<Coupon> GetByAsync(Expression<Func<Coupon, bool>> predicate);
    }
}
