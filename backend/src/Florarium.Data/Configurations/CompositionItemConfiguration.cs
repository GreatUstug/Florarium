using Florarium.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Florarium.Data.Configurations;

public class CompositionItemConfiguration : IEntityTypeConfiguration<CompositionItem>
{
    public void Configure(EntityTypeBuilder<CompositionItem> b)
    {
        b.ToTable("composition_item");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");

        b.HasOne(x => x.Composition)
            .WithMany(c => c.Items)
            .HasForeignKey(x => x.CompositionId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.Flower)
            .WithMany(f => f.CompositionItems)
            .HasForeignKey(x => x.FlowerId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => new { x.CompositionId, x.FlowerId }).IsUnique();
    }
}