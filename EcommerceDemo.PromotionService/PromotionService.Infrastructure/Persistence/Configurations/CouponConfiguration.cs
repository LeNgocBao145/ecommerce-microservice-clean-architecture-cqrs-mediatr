using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PromotionService.Domain.Entities;

namespace PromotionService.Infrastructure.Persistence.Configurations
{
    public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
    {
        public void Configure(EntityTypeBuilder<Coupon> builder)
        {
            // Configure the Coupon entity here
            builder.HasKey(c => c.Code);
            builder.Property(c => c.Type).IsRequired().HasConversion<string>();
            builder.Property(c => c.Value).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(c => c.MinOrderValue).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(c => c.StartDate).IsRequired();
            builder.Property(c => c.EndDate).IsRequired();
            builder.Property(c => c.UsageLimit).IsRequired();

            builder.ToTable(c =>
            {
                c.HasCheckConstraint("CK_Coupon_ValidDates", "[StartDate] < [EndDate]");
                c.HasCheckConstraint("CK_Coupon_NonNegativeValue", "[Value] >= 0");
                c.HasCheckConstraint("CK_Coupon_NonNegativeMinOrderValue", "[MinOrderValue] >= 0");
                c.HasCheckConstraint("CK_Coupon_NonNegativeUsageLimit", "[UsageLimit] >= 0");
            });
        }
    }
}
