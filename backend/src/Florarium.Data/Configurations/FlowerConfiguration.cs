using Florarium.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Florarium.Data.Configurations;

public class FlowerConfiguration : IEntityTypeConfiguration<Flower>
{
    public void Configure(EntityTypeBuilder<Flower> b)
    {
        b.ToTable("flower");
        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        b.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        b.Property(x => x.Variety).HasMaxLength(150);
        b.Property(x => x.LatinName).HasMaxLength(200);
        b.Property(x => x.ColorFamily).HasMaxLength(50);
        b.Property(x => x.PhotoUrl).HasMaxLength(500);

        b.Property(x => x.PricePerStem)
            .HasPrecision(10, 2);

        b.HasIndex(x => x.Name);
        b.HasIndex(x => x.ColorFamily);
    }
}