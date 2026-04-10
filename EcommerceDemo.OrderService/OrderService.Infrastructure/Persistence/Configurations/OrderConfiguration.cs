using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.Entities;

namespace OrderService.Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).ValueGeneratedOnAdd();
            builder.Property(o => o.UserId).IsRequired();
            builder.Property(o => o.DiscountAmount).HasColumnType("decimal(18,2)").HasDefaultValue(0);
            builder.Property(o => o.Subtotal).HasColumnType("decimal(18,2)").HasDefaultValue(0);
            builder.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)").HasDefaultValue(0);
            builder.Property(o => o.Notes).HasColumnType("nvarchar(255)");
            builder.Property(o => o.Status)
                    .HasConversion<string>()
                    .IsRequired();

            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Order_Status",
                    "[Status] IN ('Pending', 'Processing', 'Shipped', 'Delivered','Cancelled', 'Refunded', 'Completed')");
                t.HasCheckConstraint("CK_Order_Subtotal_NonNegative",
                    "[Subtotal] >= 0");
                t.HasCheckConstraint("CK_Order_DiscountAmount_NonNegative",
                    "[DiscountAmount] >= 0");
                t.HasCheckConstraint("CK_Order_TotalAmount_NonNegative",
                    "[TotalAmount] >= 0");
            });

            builder.HasIndex(o => o.UserId);
        }
    }
}
