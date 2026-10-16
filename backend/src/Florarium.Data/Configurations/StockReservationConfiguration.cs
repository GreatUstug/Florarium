using Florarium.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Florarium.Data.Configurations;

public class StockReservationConfiguration : IEntityTypeConfiguration<StockReservation>
{
    public void Configure(EntityTypeBuilder<StockReservation> b)
    {
        b.ToTable("stock_reservation");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");

        b.HasOne(x => x.Flower)
            .WithMany(f => f.StockReservations)
            .HasForeignKey(x => x.FlowerId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.CartItem)
            .WithMany(c => c.Reservations)
            .HasForeignKey(x => x.CartItemId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => x.FlowerId);
        b.HasIndex(x => x.CartItemId);
        b.HasIndex(x => x.ExpiresAt);
    }
}