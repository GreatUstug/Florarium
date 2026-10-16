using Florarium.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Florarium.Data.Configurations;

public class RibbonConfiguration : IEntityTypeConfiguration<Ribbon>
{
    public void Configure(EntityTypeBuilder<Ribbon> b)
    {
        b.ToTable("ribbon");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
        b.Property(x => x.Title).HasMaxLength(100).IsRequired();
        b.Property(x => x.ColorHex).HasMaxLength(7);
    }
}