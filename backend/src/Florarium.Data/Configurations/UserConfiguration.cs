using Florarium.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Florarium.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.ToTable("user");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
        b.Property(x => x.Name).HasMaxLength(200).IsRequired();
        b.Property(x => x.Email).HasMaxLength(200).IsRequired();
        b.Property(x => x.Phone).HasMaxLength(20);
        
        b.HasOne(x => x.Role)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
        
        b.HasIndex(x => x.Email).IsUnique();
    }
}