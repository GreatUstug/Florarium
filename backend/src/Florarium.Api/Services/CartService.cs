using Florarium.Api.Dtos.Cart;
using Florarium.Api.Exceptions;
using Florarium.Data;
using Florarium.Domain.Entities;
using Florarium.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Florarium.Api.Services;

public class CartService(AppDbContext db)
{
    private readonly AppDbContext _db = db;
    private const int ReservationTtlMinutes = 15;

    public async Task<CartResponse> GetCartAsync(Guid userId, CancellationToken ct)
    {
        var cart = await GetOrCreateCartAsync(userId, ct);
        return await MapToResponseAsync(cart, ct);
    }

    public async Task<CartResponse> AddItemAsync(Guid userId, AddToCartRequest request, CancellationToken ct)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        var cart = await GetOrCreateCartAsync(userId, ct);

        var composition = await _db.Compositions
            .Include(c => c.Items).ThenInclude(i => i.Flower)
            .FirstOrDefaultAsync(c => c.Id == request.CompositionId, ct);

        if (composition == null || (composition.Source == CompositionSource.Custom && composition.AuthorId != userId))
            throw new ApiException(404, "not_found", "Букет не найден");

        if (composition.Items.Any(i => !i.Flower.IsActive))
            throw new ApiException(409, "flower_inactive", "В составе букета есть снятые с продажи цветы");

        var existingItem = cart.Items.FirstOrDefault(i => i.CompositionId == request.CompositionId);
        int addedQty = request.Qty;
        int newTotalQty = (existingItem?.Qty ?? 0) + addedQty;

        var flowerIds = composition.Items.Select(i => i.FlowerId).ToArray();
        var stocks = await _db.Stocks
            .FromSqlInterpolated($"SELECT *, xmin FROM stock WHERE flower_id = ANY({flowerIds}) FOR UPDATE")
            .ToDictionaryAsync(s => s.FlowerId, ct);

        foreach (var compItem in composition.Items)
        {
            var stock = stocks[compItem.FlowerId];
            int neededForAdd = compItem.Qty * addedQty;
            int freeStock = stock.Available - stock.Reserved;

            if (freeStock < neededForAdd)
            {
                throw new ApiException(409, "out_of_stock", 
                    $"Недостаточно цветов {compItem.Flower.Name} на складе (требуется ещё {neededForAdd}, свободно {freeStock})");
            }
        }

        foreach (var compItem in composition.Items)
        {
            stocks[compItem.FlowerId].Reserved += compItem.Qty * addedQty;
        }

        DateTime expiresAt = DateTime.UtcNow.AddMinutes(ReservationTtlMinutes);

        if (existingItem != null)
        {
            existingItem.Qty = newTotalQty;
            existingItem.UnitPrice = composition.Price;

            var reservations = await _db.StockReservations.Where(r => r.CartItemId == existingItem.Id).ToListAsync(ct);
            foreach (var res in reservations)
            {
                var compItem = composition.Items.First(i => i.FlowerId == res.FlowerId);
                res.Qty = compItem.Qty * newTotalQty;
                res.ExpiresAt = expiresAt;
            }
        }
        else
        {
            existingItem = new CartItem
            {
                CartId = cart.Id,
                CompositionId = composition.Id,
                Qty = newTotalQty,
                UnitPrice = composition.Price
            };
            cart.Items.Add(existingItem);

            foreach (var compItem in composition.Items)
            {
                _db.StockReservations.Add(new StockReservation
                {
                    FlowerId = compItem.FlowerId,
                    CartItem = existingItem,
                    Qty = compItem.Qty * newTotalQty,
                    ExpiresAt = expiresAt
                });
            }
        }

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return await MapToResponseAsync(cart, ct);
    }

    public async Task<CartResponse> UpdateItemQtyAsync(Guid userId, Guid itemId, int newQty, CancellationToken ct)
    {
        if (newQty <= 0) return await RemoveItemAsync(userId, itemId, ct);

        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        var cart = await GetOrCreateCartAsync(userId, ct);
        var cartItem = cart.Items.FirstOrDefault(i => i.Id == itemId);
        if (cartItem == null) throw new ApiException(404, "not_found", "Позиция не найдена в корзине");

        var composition = await _db.Compositions
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == cartItem.CompositionId, ct);

        int delta = newQty - cartItem.Qty;

        if (delta != 0)
        {
            var flowerIds = composition!.Items.Select(i => i.FlowerId).ToArray();
            var stocks = await _db.Stocks
                .FromSqlInterpolated($"SELECT *, xmin FROM stock WHERE flower_id = ANY({flowerIds}) FOR UPDATE")
                .ToDictionaryAsync(s => s.FlowerId, ct);

            if (delta > 0)
            {
                foreach (var compItem in composition.Items)
                {
                    var stock = stocks[compItem.FlowerId];
                    int needed = compItem.Qty * delta;
                    int free = stock.Available - stock.Reserved;

                    if (free < needed)
                        throw new ApiException(409, "out_of_stock", "Недостаточно цветов на складе для увеличения количества");
                }
            }

            foreach (var compItem in composition.Items)
            {
                stocks[compItem.FlowerId].Reserved += compItem.Qty * delta;
            }

            var reservations = await _db.StockReservations.Where(r => r.CartItemId == cartItem.Id).ToListAsync(ct);
            DateTime expiresAt = DateTime.UtcNow.AddMinutes(ReservationTtlMinutes);

            foreach (var res in reservations)
            {
                var compItem = composition.Items.First(i => i.FlowerId == res.FlowerId);
                res.Qty = compItem.Qty * newQty;
                res.ExpiresAt = expiresAt;
            }

            cartItem.Qty = newQty;
            await _db.SaveChangesAsync(ct);
        }

        await tx.CommitAsync(ct);
        return await MapToResponseAsync(cart, ct);
    }

    public async Task<CartResponse> RemoveItemAsync(Guid userId, Guid itemId, CancellationToken ct)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        var cart = await GetOrCreateCartAsync(userId, ct);
        var cartItem = cart.Items.FirstOrDefault(i => i.Id == itemId);

        if (cartItem != null)
        {
            await ReleaseCartItemReservationAsync(cartItem.Id, ct);
            _db.CartItems.Remove(cartItem);
            await _db.SaveChangesAsync(ct);
        }

        await tx.CommitAsync(ct);
        return await MapToResponseAsync(cart, ct);
    }

    public async Task ClearCartAsync(Guid userId, CancellationToken ct)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        var cart = await GetOrCreateCartAsync(userId, ct);
        foreach (var item in cart.Items)
        {
            await ReleaseCartItemReservationAsync(item.Id, ct);
        }

        _db.CartItems.RemoveRange(cart.Items);
        await _db.SaveChangesAsync(ct);

        await tx.CommitAsync(ct);
    }

    private async Task ReleaseCartItemReservationAsync(Guid cartItemId, CancellationToken ct)
    {
        var reservations = await _db.StockReservations
            .Where(r => r.CartItemId == cartItemId)
            .ToListAsync(ct);

        if (reservations.Count == 0) return;

        var flowerIds = reservations.Select(r => r.FlowerId).Distinct().ToArray();
        var stocks = await _db.Stocks
            .FromSqlInterpolated($"SELECT *, xmin FROM stock WHERE flower_id = ANY({flowerIds}) FOR UPDATE")
            .ToDictionaryAsync(s => s.FlowerId, ct);

        foreach (var res in reservations)
        {
            if (stocks.TryGetValue(res.FlowerId, out var stock))
            {
                stock.Reserved = Math.Max(0, stock.Reserved - res.Qty);
            }
        }

        _db.StockReservations.RemoveRange(reservations);
    }

    private async Task<Cart> GetOrCreateCartAsync(Guid userId, CancellationToken ct)
    {
        var cart = await _db.Carts
            .Include(c => c.Items).ThenInclude(i => i.Composition)
            .FirstOrDefaultAsync(c => c.CustomerId == userId, ct);

        if (cart == null)
        {
            cart = new Cart { CustomerId = userId };
            _db.Carts.Add(cart);
            await _db.SaveChangesAsync(ct);
        }

        return cart;
    }

    private async Task<CartResponse> MapToResponseAsync(Cart cart, CancellationToken ct)
    {
        var itemIds = cart.Items.Select(i => i.Id).ToList();

        var items = cart.Items.Select(i => new CartItemDto
        {
            Id = i.Id,
            Composition = new CartCompositionDto
            {
                Id = i.Composition.Id,
                Title = i.Composition.Title,
                PhotoUrl = i.Composition.PhotoUrl,
                Source = i.Composition.Source.ToString().ToLower()
            },
            Qty = i.Qty,
            UnitPrice = i.UnitPrice,
            LineTotal = i.Qty * i.UnitPrice,
            Status = "reserved",
            MaxQty = 50
        }).ToList();

        var subtotal = items.Sum(i => i.LineTotal);

        var reservedUntil = await _db.StockReservations
            .Where(r => itemIds.Contains(r.CartItemId))
            .MinAsync(r => (DateTime?)r.ExpiresAt, ct);

        return new CartResponse
        {
            Items = items,
            Count = items.Count,
            Subtotal = subtotal,
            Total = subtotal,
            ReservedUntil = reservedUntil
        };
    }
}