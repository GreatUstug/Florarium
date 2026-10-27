using Florarium.Domain.Entities;
using Florarium.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Florarium.Data.Seed;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Roles.AnyAsync()) return;

        // --- роли ---
        var adminRole   = new Role { Name = "admin",   Rights = 0b1111 };
        var floristRole = new Role { Name = "florist", Rights = 0b0011 };
        var clientRole  = new Role { Name = "client",  Rights = 0b0001 };
        db.Roles.AddRange(adminRole, floristRole, clientRole);

        // --- пользователи ---
        var admin = new User { Role = adminRole, Name = "Администратор", Email = "admin@florarium.local", PasswordHash = "DUMMY_HASH" };
        var florist = new User { Role = floristRole, Name = "Мария Флорист", Email = "florist@florarium.local", PasswordHash = "DUMMY_HASH" };
        var client = new User { Role = clientRole, Name = "Иван Петров", Email = "client@florarium.local", PasswordHash = "DUMMY_HASH", Phone = "+7 900 123-45-67" };
        db.Users.AddRange(admin, florist, client);

        // --- цветы + остатки ---
        var flowers = new List<Flower>
        {
            new() { Name = "Роза Red Naomi",       PricePerStem = 180m, ColorFamily = "красный", Stock = new Stock { Available = 200, UpdatedAt = DateTime.UtcNow } },
            new() { Name = "Роза Avalanche",       PricePerStem = 190m, ColorFamily = "белый",   Stock = new Stock { Available = 150, UpdatedAt = DateTime.UtcNow } },
            new() { Name = "Пион Sarah Bernhardt", PricePerStem = 450m, ColorFamily = "розовый", Stock = new Stock { Available = 40,  UpdatedAt = DateTime.UtcNow } },
            new() { Name = "Тюльпан Strong Gold",  PricePerStem = 80m,  ColorFamily = "жёлтый",  Stock = new Stock { Available = 300, UpdatedAt = DateTime.UtcNow } },
            new() { Name = "Эустома Alissa",       PricePerStem = 220m, ColorFamily = "белый",   Stock = new Stock { Available = 80,  UpdatedAt = DateTime.UtcNow } },
            new() { Name = "Гортензия Magical",    PricePerStem = 550m, ColorFamily = "голубой", Stock = new Stock { Available = 25,  UpdatedAt = DateTime.UtcNow } },
            new() { Name = "Хризантема кустовая",  PricePerStem = 120m, ColorFamily = "белый",   Stock = new Stock { Available = 120, UpdatedAt = DateTime.UtcNow } },
            new() { Name = "Гипсофила",            PricePerStem = 60m,  ColorFamily = "белый",   Stock = new Stock { Available = 500, UpdatedAt = DateTime.UtcNow } },
        };
        db.Flowers.AddRange(flowers);

        // --- упаковки ---
        var packKraft = new Packaging { Title = "Крафт-бумага",       Price = 100m };
        var packBox   = new Packaging { Title = "Подарочная коробка", Price = 350m };
        db.Packagings.AddRange(packKraft, packBox);

        // --- ленты ---
        var ribbonPink  = new Ribbon { Title = "Атласная роза",  ColorHex = "#FF6B9D" };
        var ribbonWhite = new Ribbon { Title = "Атласная белая", ColorHex = "#FFFFFF" };
        db.Ribbons.AddRange(ribbonPink, ribbonWhite);

        await db.SaveChangesAsync();

        // --- шаблоны букетов (composition с source=Template) ---
        var springMix = new Composition
        {
            Author = florist,
            Packaging = packKraft,
            Ribbon = ribbonPink,
            Source = CompositionSource.Template,
            Title = "Весенний микс",
            Items =
            [
                new() { Flower = flowers[3], Qty = 7 }, // тюльпаны
                new() { Flower = flowers[7], Qty = 3 }, // гипсофила
            ]
        };
        var redClassic = new Composition
        {
            Author = florist,
            Packaging = packBox,
            Ribbon = ribbonWhite,
            Source = CompositionSource.Template,
            Title = "Красная классика",
            Items =
            [
                new() { Flower = flowers[0], Qty = 15 }, // розы
                new() { Flower = flowers[7], Qty = 5 },  // гипсофила
            ]
        };
        db.Compositions.AddRange(springMix, redClassic);

        await db.SaveChangesAsync();

        // пересчёт цены: состав + упаковка
        springMix.Price = springMix.Items.Sum(i => i.Qty * i.Flower.PricePerStem)
                          + springMix.Packaging!.Price;
        redClassic.Price = redClassic.Items.Sum(i => i.Qty * i.Flower.PricePerStem)
                          + redClassic.Packaging!.Price;

        // --- один завершённый заказ ---
        var order1 = new Order
        {
            Number = "ORD-2026-0001",
            Customer = client,
            Status = OrderStatus.Done,
            Items =
            [
                new() { Composition = springMix, Qty = 1, UnitPrice = springMix.Price }
            ],
            Delivery = new Delivery
            {
                Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-5)),
                RecipientName = "Иван Петров",
                Street = "ул. Ленина, 10",
                Status = DeliveryStatus.Delivered,
                DeliveredAt = DateTime.UtcNow.AddDays(-5)
            },
            Payments =
            [
                new()
                {
                    Method = PaymentMethod.Card,
                    Status = PaymentStatus.Paid,
                    Amount = springMix.Price,
                    PaidAt = DateTime.UtcNow.AddDays(-6)
                }
            ]
        };
        order1.Subtotal = order1.Items.Sum(i => i.Qty * i.UnitPrice);
        order1.Discount = 0;
        order1.Total = order1.Subtotal - order1.Discount;

        db.Orders.Add(order1);
        await db.SaveChangesAsync();
    }
}