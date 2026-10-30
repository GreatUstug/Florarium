using Florarium.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Florarium.Data.Configurations;

public class RatingConfiguration : IEntityTypeConfiguration<Rating>
{
    public void Configure(EntityTypeBuilder<Rating> b)
    {
        b.ToTable("rating");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
        b.Property(x => x.Text).HasMaxLength(2000);

        b.HasOne(x => x.OrderItem)
            .WithOne(oi => oi.Rating)
            .HasForeignKey<Rating>(x => x.OrderItemId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.Customer)
            .WithMany(u => u.Ratings)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => x.OrderItemId).IsUnique();
        b.HasIndex(x => x.CustomerId);
    }
}