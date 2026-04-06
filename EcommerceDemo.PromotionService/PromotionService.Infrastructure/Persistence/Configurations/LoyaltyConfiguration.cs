using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PromotionService.Domain.Entities;

namespace PromotionService.Infrastructure.Persistence.Configurations
{
    public class LoyaltyConfiguration : IEntityTypeConfiguration<Loyalty>
    {
        public void Configure(EntityTypeBuilder<Loyalty> builder)
        {
            builder.HasKey(l => l.UserId);
            builder.Property(l => l.Points).IsRequired();
            builder.Property(l => l.Rank).IsRequired().HasConversion<string>();
            builder.ToTable(l =>
            {
                l.HasCheckConstraint("CK_Loyalty_NonNegativePoints", "[Points] >= 0");
            });
        }
    }
}
