using Florarium.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Florarium.Data.Configurations;

public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> b)
    {
        b.ToTable("cart");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
        
        b.HasOne(x => x.Customer)
            .WithOne(u => u.Cart) 
            .HasForeignKey<Cart>(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => x.CustomerId).IsUnique();
    }
}