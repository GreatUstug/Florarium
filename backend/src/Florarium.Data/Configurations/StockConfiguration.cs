using Florarium.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Florarium.Data.Configurations;

public class StockConfiguration : IEntityTypeConfiguration<Stock>
{
    public void Configure(EntityTypeBuilder<Stock> b)
    {
        b.ToTable("stock");

        b.HasKey(x => x.FlowerId);

        b.Property(x => x.Available).HasDefaultValue(0);
        b.Property(x => x.Reserved).HasDefaultValue(0);

        b.HasOne(x => x.Flower)
            .WithOne(f => f.Stock)
            .HasForeignKey<Stock>(x => x.FlowerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
