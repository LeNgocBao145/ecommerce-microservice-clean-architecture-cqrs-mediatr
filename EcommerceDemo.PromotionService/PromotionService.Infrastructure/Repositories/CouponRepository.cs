using PromotionService.Domain.Entities;
using PromotionService.Domain.Interfaces;
using PromotionService.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace PromotionService.Infrastructure.Repositories
{
    public class CouponRepository(AppDbContext context) : ICouponRepository
    {
        public async Task<Coupon> CreateAsync(Coupon entity)
        {
            context.Coupons.Add(entity);
            await context.SaveChangesAsync();
            return entity;
        }

        public async Task<Coupon> UpdateAsync(Coupon entity)
        {
            context.Coupons.Update(entity);
            await context.SaveChangesAsync();
            return entity;
        }

        public async Task<Coupon> GetByAsync(Expression<Func<Coupon, bool>> predicate)
        {
            return await context.Coupons.FirstOrDefaultAsync();
        }
    }
}
