using Florarium.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Florarium.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<Flower> Flowers => Set<Flower>();
    public DbSet<Stock> Stocks => Set<Stock>();
    public DbSet<SupplyBatch> SupplyBatches => Set<SupplyBatch>();
    public DbSet<StockReservation> StockReservations => Set<StockReservation>();
    public DbSet<Packaging> Packagings => Set<Packaging>();
    public DbSet<Ribbon> Ribbons => Set<Ribbon>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Composition> Compositions => Set<Composition>();
    public DbSet<CompositionItem> CompositionItems => Set<CompositionItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Delivery> Deliveries => Set<Delivery>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Rating> Ratings => Set<Rating>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}