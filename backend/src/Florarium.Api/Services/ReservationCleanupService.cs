using Florarium.Data;

using Microsoft.EntityFrameworkCore;

namespace Florarium.Api.Services;

/// <summary>Сервис ReservationCleanupService.</summary>
public class ReservationCleanupService(IServiceProvider services, ILogger<ReservationCleanupService> logger) : BackgroundService
{
    private readonly IServiceProvider _services = services;
    private readonly ILogger<ReservationCleanupService> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupExpiredReservationsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при выполнении фоновой очистки резервов");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private async Task CleanupExpiredReservationsAsync(CancellationToken ct)
    {
        using var scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await using var tx = await db.Database.BeginTransactionAsync(ct);

        var now = DateTime.UtcNow;

        var expired = await db.StockReservations
            .Include(r => r.CartItem)
            .Where(r => r.ExpiresAt < now)
            .ToListAsync(ct);

        if (expired.Count == 0) return;

        var flowerIds = expired.Select(r => r.FlowerId).Distinct().ToArray();

        var stocks = await db.Stocks
            .FromSqlInterpolated($"SELECT *, xmin FROM stock WHERE flower_id = ANY({flowerIds}) ORDER BY flower_id FOR UPDATE")
            .ToDictionaryAsync(s => s.FlowerId, ct);

        foreach (var res in expired)
        {
            if (stocks.TryGetValue(res.FlowerId, out var stock))
            {
                stock.Reserved = Math.Max(0, stock.Reserved - res.Qty);
            }
        }

        var cartItemsToRemove = expired.Select(r => r.CartItem).Where(ci => ci != null).Distinct().ToList();
        db.CartItems.RemoveRange(cartItemsToRemove);

        db.StockReservations.RemoveRange(expired);

        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        _logger.LogInformation("Очищено {Count} просроченных резервов и удалено {CartCount} позиций из корзин.", expired.Count, cartItemsToRemove.Count);
    }
}
