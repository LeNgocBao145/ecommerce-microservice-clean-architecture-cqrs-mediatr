using Microsoft.EntityFrameworkCore;
using PromotionService.Domain.Entities;

namespace PromotionService.Infrastructure.Persistence
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<Loyalty> Loyalties { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
