using Florarium.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Florarium.Data;

/// <summary>Контекст базы данных Florarium для доступа к сущностям и миграциям.</summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    /// <summary>Роли пользователей и права доступа.</summary>
    public DbSet<Role> Roles => Set<Role>();
    /// <summary>Пользователи интернет-магазина.</summary>
    public DbSet<User> Users => Set<User>();
    /// <summary>Адреса доставки пользователей.</summary>
    public DbSet<Address> Addresses => Set<Address>();
    /// <summary>Каталог цветов.</summary>
    public DbSet<Flower> Flowers => Set<Flower>();
    /// <summary>Остатки цветов на складе.</summary>
    public DbSet<Stock> Stocks => Set<Stock>();
    /// <summary>Партии поступления цветов.</summary>
    public DbSet<SupplyBatch> SupplyBatches => Set<SupplyBatch>();
    /// <summary>Резервирование остатков при оформлении корзины и заказа.</summary>
    public DbSet<StockReservation> StockReservations => Set<StockReservation>();
    /// <summary>Справочник упаковки.</summary>
    public DbSet<Packaging> Packagings => Set<Packaging>();
    /// <summary>Справочник лент.</summary>
    public DbSet<Ribbon> Ribbons => Set<Ribbon>();
    /// <summary>Корзины пользователей.</summary>
    public DbSet<Cart> Carts => Set<Cart>();
    /// <summary>Позиции в корзинах.</summary>
    public DbSet<CartItem> CartItems => Set<CartItem>();
    /// <summary>Составленные пользователями букеты.</summary>
    public DbSet<Composition> Compositions => Set<Composition>();
    /// <summary>Цветы и их количество в составе букета.</summary>
    public DbSet<CompositionItem> CompositionItems => Set<CompositionItem>();
    /// <summary>Заказы покупателей.</summary>
    public DbSet<Order> Orders => Set<Order>();
    /// <summary>Позиции заказанных букетов.</summary>
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    /// <summary>Доставка заказа.</summary>
    public DbSet<Delivery> Deliveries => Set<Delivery>();
    /// <summary>Платежные операции заказа.</summary>
    public DbSet<Payment> Payments => Set<Payment>();
    /// <summary>Отзывы покупателей на позиции заказов.</summary>
    public DbSet<Rating> Ratings => Set<Rating>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasSequence<int>("order_number_seq").StartsAt(1000);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}