using Microsoft.EntityFrameworkCore;
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

        public async Task<Coupon> UpdateCouponAsync(Coupon entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            context.Coupons.Update(entity);
            await context.SaveChangesAsync();
            return entity;
        }

        public async Task<Loyalty> UpdateLoyaltyAsync(Loyalty entity)
        {
            context.Loyalties.Update(entity);
            await context.SaveChangesAsync();
            return entity;
        }

        public async Task<Coupon?> GetCouponByAsync(Expression<Func<Coupon, bool>> predicate)
        {
            return await context.Coupons.FirstOrDefaultAsync(predicate);
        }

        public async Task<Loyalty?> GetLoyaltyByAsync(Expression<Func<Loyalty, bool>> predicate)
        {
            return await context.Loyalties.FirstOrDefaultAsync(predicate);
        }

        public async Task<bool> CouponCodeExistsAsync(string code)
        {
            return await context.Coupons.AnyAsync(c => c.Code == code);
        }
    }
}
