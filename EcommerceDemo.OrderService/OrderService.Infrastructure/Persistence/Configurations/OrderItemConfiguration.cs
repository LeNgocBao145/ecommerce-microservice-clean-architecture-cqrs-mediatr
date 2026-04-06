using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.Entities;

namespace OrderService.Infrastructure.Persistence.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasKey(oi => oi.Id);
            builder.Property(oi => oi.Id).ValueGeneratedOnAdd();
            builder.Property(oi => oi.OrderId).IsRequired();
            builder.Property(oi => oi.ProductId).IsRequired();
            builder.Property(oi => oi.Quantity).IsRequired();
            builder.Property(oi => oi.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(oi => oi.TotalPrice).HasColumnType("decimal(18,2)").IsRequired();

            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_OrderItem_Quantity", "[Quantity] > 0");
                t.HasCheckConstraint("CK_OrderItem_UnitPrice", "[UnitPrice] >= 0");
                t.HasCheckConstraint("CK_OrderItem_TotalPrice", "[TotalPrice] >= 0");
            });

            builder.HasOne(oi => oi.Order)
                   .WithMany(o => o.OrderItems)
                   .HasForeignKey(oi => oi.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
