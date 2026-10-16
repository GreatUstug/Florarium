using Florarium.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Florarium.Data.Configurations;

public class DeliveryConfiguration : IEntityTypeConfiguration<Delivery>
{
    public void Configure(EntityTypeBuilder<Delivery> b)
    {
        b.ToTable("delivery");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
        b.Property(x => x.RecipientName).HasMaxLength(200);
        b.Property(x => x.Slot).HasMaxLength(50);
        b.Property(x => x.CourierNote).HasMaxLength(500);

        b.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        b.HasOne(x => x.Order)
            .WithOne(o => o.Delivery)
            .HasForeignKey<Delivery>(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => x.OrderId).IsUnique();
    }
}