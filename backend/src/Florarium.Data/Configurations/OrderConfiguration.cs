using Florarium.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Florarium.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> b)
    {
        b.ToTable("order");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
        b.Property(x => x.Number).HasMaxLength(50).IsRequired();
        b.Property(x => x.Subtotal).HasPrecision(10, 2);
        b.Property(x => x.Discount).HasPrecision(10, 2);
        b.Property(x => x.Total).HasPrecision(10, 2);

        b.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        b.HasOne(x => x.Customer)
            .WithMany(u => u.Orders)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => x.Number).IsUnique();
        b.HasIndex(x => x.CustomerId);
        b.HasIndex(x => x.Status);
    }
}