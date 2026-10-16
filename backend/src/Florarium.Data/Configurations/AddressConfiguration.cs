using Florarium.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Florarium.Data.Configurations;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> b)
    {
        b.ToTable("address");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
        b.Property(x => x.Label).HasMaxLength(50);
        b.Property(x => x.Street).HasMaxLength(300).IsRequired();
        b.Property(x => x.Apartment).HasMaxLength(20);
        b.Property(x => x.Entrance).HasMaxLength(20);
        b.Property(x => x.Floor).HasMaxLength(20);
        b.Property(x => x.Intercom).HasMaxLength(20);

        b.HasOne(x => x.Customer)
            .WithMany(u => u.Addresses)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => x.CustomerId);
    }
}