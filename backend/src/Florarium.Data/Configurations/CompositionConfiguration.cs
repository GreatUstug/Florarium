using Florarium.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Florarium.Data.Configurations;

public class CompositionConfiguration : IEntityTypeConfiguration<Composition>
{
    public void Configure(EntityTypeBuilder<Composition> b)
    {
        b.ToTable("composition");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
        b.Property(x => x.Price).HasPrecision(10, 2);
        b.Property(x => x.CardText).HasMaxLength(500);
        b.Property(x => x.PhotoUrl).HasMaxLength(500);

        b.Property(x => x.Source)
            .HasConversion<string>()
            .HasMaxLength(20);

        b.HasOne(x => x.Author)
            .WithMany(u => u.Compositions)
            .HasForeignKey(x => x.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Packaging)
            .WithMany(p => p.Compositions)
            .HasForeignKey(x => x.PackagingId)
            .OnDelete(DeleteBehavior.SetNull);

        b.HasOne(x => x.Ribbon)
            .WithMany(r => r.Compositions)
            .HasForeignKey(x => x.RibbonId)
            .OnDelete(DeleteBehavior.SetNull);

        b.HasIndex(x => x.AuthorId);
        b.HasIndex(x => x.Source);
    }
}