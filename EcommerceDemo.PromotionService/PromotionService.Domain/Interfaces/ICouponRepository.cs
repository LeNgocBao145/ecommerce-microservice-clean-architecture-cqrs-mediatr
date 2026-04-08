using PromotionService.Domain.Entities;
using System.Linq.Expressions;

namespace PromotionService.Domain.Interfaces
{
    public interface ICouponRepository
    {
        Task<Coupon> CreateAsync(Coupon entity);
        Task<Coupon> UpdateCouponAsync(Coupon entity);
        Task<Loyalty> UpdateLoyaltyAsync(Loyalty entity);
        Task<Coupon?> GetCouponByAsync(Expression<Func<Coupon, bool>> predicate);
        Task<Loyalty?> GetLoyaltyByAsync(Expression<Func<Loyalty, bool>> predicate);
    }
}
