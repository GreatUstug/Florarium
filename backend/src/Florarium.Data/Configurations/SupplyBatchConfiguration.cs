using Florarium.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Florarium.Data.Configurations;

public class SupplyBatchConfiguration : IEntityTypeConfiguration<SupplyBatch>
{
    public void Configure(EntityTypeBuilder<SupplyBatch> b)
    {
        b.ToTable("supply_batch");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
        b.Property(x => x.PurchasePrice).HasPrecision(10, 2);

        b.HasOne(x => x.Flower)
            .WithMany(f => f.SupplyBatches)
            .HasForeignKey(x => x.FlowerId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => x.FlowerId);
    }
}