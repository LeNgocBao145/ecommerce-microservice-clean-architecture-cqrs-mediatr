using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.Entities;

namespace OrderService.Infrastructure.Persistence.Configurations
{
    public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.HasKey(ci => ci.Id);
            builder.Property(ci => ci.Id).ValueGeneratedOnAdd();
            builder.Property(ci => ci.CartId).IsRequired();
            builder.Property(ci => ci.ProductId).IsRequired();
            builder.Property(ci => ci.Quantity).IsRequired();
            builder.Property(ci => ci.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_CartItem_Quantity", "[Quantity] > 0");
                t.HasCheckConstraint("CK_CartItem_UnitPrice", "[UnitPrice] >= 0");
            });

            builder.HasOne(ci => ci.Cart)
                   .WithMany(c => c.CartItems)
                   .HasForeignKey(ci => ci.CartId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
