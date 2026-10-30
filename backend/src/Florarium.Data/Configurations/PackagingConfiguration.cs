using Florarium.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Florarium.Data.Configurations;

public class PackagingConfiguration : IEntityTypeConfiguration<Packaging>
{
    public void Configure(EntityTypeBuilder<Packaging> b)
    {
        b.ToTable("packaging");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
        b.Property(x => x.Title).HasMaxLength(100).IsRequired();
        b.Property(x => x.Price).HasPrecision(10, 2);
    }
}