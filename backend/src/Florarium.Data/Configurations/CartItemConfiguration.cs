using Florarium.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Florarium.Data.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> b)
    {
        b.ToTable("cart_item");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
        b.Property(x => x.UnitPrice).HasPrecision(10, 2);

        b.HasOne(x => x.Cart)
            .WithMany(c => c.Items)
            .HasForeignKey(x => x.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.Composition)
            .WithMany(c => c.CartItems)
            .HasForeignKey(x => x.CompositionId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => x.CartId);
        b.HasIndex(x => x.CompositionId);
    }
}