using Florarium.Api.Dtos.Compositions;
using Florarium.Data;
using Microsoft.EntityFrameworkCore;

using Florarium.Api.Dtos.Compositions;
using Florarium.Api.Exceptions;
using Florarium.Domain.Entities;
using Florarium.Domain.Enums;
using Florarium.Api.Dtos.Bouquets;
using Florarium.Api.Dtos.Common;

namespace Florarium.Api.Services;

/// <summary>Сервис CompositionsService.</summary>
public class CompositionsService(AppDbContext db)
{
    private readonly AppDbContext _db = db;

    /// <summary>Выполняет QuoteAsync.</summary>
    /// <exception cref="Florarium.Api.Exceptions.ApiException">Возникает при нарушении бизнес-правил операции.</exception>
    public async Task<QuoteResponse> QuoteAsync(QuoteRequest request, CancellationToken ct)
    {
        var flowerIds = request.Items.Select(i => i.FlowerId).Distinct().ToList();

        var flowers = await _db.Flowers
            .AsNoTracking()
            .Include(f => f.Stock)
            .Where(f => flowerIds.Contains(f.Id))
            .ToDictionaryAsync(f => f.Id, f => f, ct);

        var response = new QuoteResponse();

        if (request.PackagingId.HasValue)
        {
            var packaging = await _db.Packagings
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == request.PackagingId.Value, ct);

            if (packaging != null)
                response.PackagingPrice = packaging.Price;
        }

        foreach (var item in request.Items)
        {
            if (!flowers.TryGetValue(item.FlowerId, out var flower))
            {
                throw new ArgumentException($"Цветок с ID {item.FlowerId} не найден");
            }

            var lineTotal = item.Qty * flower.PricePerStem;
            var free = flower.Stock.Available - flower.Stock.Reserved;

            response.Items.Add(new QuoteItemResponse
            {
                FlowerId = flower.Id,
                FlowerName = flower.Name,
                Qty = item.Qty,
                PricePerStem = flower.PricePerStem,
                LineTotal = lineTotal
            });

            response.FlowersTotal += lineTotal;

            if (item.Qty > free)
            {
                response.Warnings.Add(new QuoteWarningDto
                {
                    FlowerId = flower.Id,
                    Requested = item.Qty,
                    Available = free
                });
            }
        }

        response.Total = response.FlowersTotal + response.PackagingPrice;

        return response;
    }

    /// <summary>Выполняет CreateAsync.</summary>
    /// <exception cref="Florarium.Api.Exceptions.ApiException">Возникает при нарушении бизнес-правил операции.</exception>
    public async Task<CompositionDetailDto> CreateAsync(
    SaveCompositionRequest request, Guid authorId, CancellationToken ct)
    {
        var flowerIds = request.Items.Select(i => i.FlowerId).ToList();
        if (flowerIds.Count != flowerIds.Distinct().Count())
            throw new ApiException(400, "validation_failed", "Цветы в букете не должны повторяться");

        var flowers = await _db.Flowers.AsNoTracking().Where(f => flowerIds.Contains(f.Id)).ToDictionaryAsync(f => f.Id, ct);

        foreach (var item in request.Items)
        {
            if (!flowers.TryGetValue(item.FlowerId, out var flower))
                throw new ApiException(400, "validation_failed", $"Цветок {item.FlowerId} не найден");
            if (!flower.IsActive)
                throw new ApiException(409, "flower_inactive", $"Цветок {flower.Name} снят с продажи");
        }

        var composition = new Composition
        {
            AuthorId = authorId,
            Source = CompositionSource.Custom,
            CardText = request.CardText,
            PackagingId = request.PackagingId,
            RibbonId = request.RibbonId,
            CreatedAt = DateTime.UtcNow
        };

        decimal flowersTotal = 0;
        foreach (var item in request.Items)
        {
            var flower = flowers[item.FlowerId];
            flowersTotal += item.Qty * flower.PricePerStem;

            composition.Items.Add(new CompositionItem
            {
                FlowerId = flower.Id,
                Qty = item.Qty
            });
        }

        // 5. Упаковка (если есть)
        // TODO: if (request.PackagingId.HasValue) → загрузить, проверить, добавить цену
        decimal packagingPrice = 0;
        if (request.PackagingId.HasValue)
        {
            var packaging = await _db.Packagings.FirstOrDefaultAsync(p => p.Id == request.PackagingId, ct)
            ?? throw new ApiException(400, "validation_failed", "Упаковка не найдена");

            packagingPrice = packaging.Price;
        }
        // 6. Итоговая цена и сохранение
        // TODO: composition.Price = flowersTotal + packagingPrice
        // _db.Compositions.Add(composition);
        // await _db.SaveChangesAsync(ct);

        composition.Price = flowersTotal + packagingPrice;

        _db.Compositions.Add(composition);
        await _db.SaveChangesAsync(ct);

        // 7. Возврат (GetByIdAsync напишем на следующем шаге, пока оставь заглушку)
        // TODO: return await GetByIdAsync(composition.Id, authorId, ct) ?? throw ...

        return await GetByIdAsync(composition.Id, authorId, ct)
            ?? throw new Exception("Ошибка при создании состава");
    }

    /// <summary>Выполняет GetByIdAsync.</summary>
    /// <exception cref="Florarium.Api.Exceptions.ApiException">Возникает при нарушении бизнес-правил операции.</exception>
    public async Task<CompositionDetailDto?> GetByIdAsync(
        Guid id, Guid? currentUserId, CancellationToken ct)
    {
        var c = await _db.Compositions
            .AsNoTracking()
            .Include(x => x.Author)
            .Include(x => x.Packaging)
            .Include(x => x.Ribbon)
            .Include(x => x.Items).ThenInclude(i => i.Flower).ThenInclude(f => f.Stock)
            .Include(x => x.CartItems)
            .Include(x => x.OrderItems)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (c == null) return null;

        if (c.Source == CompositionSource.Custom && c.AuthorId != currentUserId) return null;

        bool isEditable = c.Source == CompositionSource.Custom
                          && c.AuthorId == currentUserId
                          && c.CartItems.Count == 0
                          && c.OrderItems.Count == 0;

        return new CompositionDetailDto
        {
            Id = c.Id,
            Title = c.Title,
            PhotoUrl = c.PhotoUrl,
            Source = c.Source.ToString().ToLower(System.Globalization.CultureInfo.CurrentCulture),
            Price = c.Price,
            CardText = c.CardText,
            IsEditable = isEditable,
            PriceBreakdown = new PriceBreakdownDto
            {
                Flowers = c.Items.Sum(i => i.Qty * i.Flower.PricePerStem),
                Packaging = c.Packaging?.Price ?? 0
            },
            Items = [.. c.Items.Select(i => new BouquetItemDto
            {
                Qty = i.Qty,
                LineTotal = i.Qty * i.Flower.PricePerStem,
                Flower = new FlowerDto
                {
                    Id = i.Flower.Id,
                    Name = i.Flower.Name,
                    Variety = i.Flower.Variety,
                    ColorFamily = i.Flower.ColorFamily,
                    PhotoUrl = i.Flower.PhotoUrl,
                    PricePerStem = i.Flower.PricePerStem
                }
            })],
            Packaging = c.Packaging != null ? new PackagingDto
            {
                Id = c.Packaging.Id,
                Title = c.Packaging.Title,
                Price = c.Packaging.Price
            } : null,
            Ribbon = c.Ribbon != null ? new RibbonDto
            {
                Id = c.Ribbon.Id,
                Title = c.Ribbon.Title,
                ColorHex = c.Ribbon.ColorHex
            } : null,
            Availability = new BouquetDetailAvailabilityDto
            {
                MaxQty = c.Items.Any(i => !i.Flower.IsActive) ? 0 : c.Items.Min(i => (i.Flower.Stock.Available - i.Flower.Stock.Reserved) / i.Qty),
                InStock = !c.Items.Any(i => !i.Flower.IsActive) && c.Items.Min(i => (i.Flower.Stock.Available - i.Flower.Stock.Reserved) / i.Qty) > 0,
                BackInStockAt = c.Items.Where(i => (i.Flower.Stock.Available - i.Flower.Stock.Reserved) < i.Qty).Max(i => (DateOnly?)i.Flower.Stock.BackInStockAt)
            },
            Rating = new BouquetRatingDto()
        };
    }

    /// <summary>Выполняет UpdateAsync.</summary>
    /// <exception cref="Florarium.Api.Exceptions.ApiException">Возникает при нарушении бизнес-правил операции.</exception>
    public async Task<CompositionDetailDto> UpdateAsync(Guid id, SaveCompositionRequest request, Guid authorId, CancellationToken ct)
    {
        var composition = await _db.Compositions
            .Include(c => c.Items)
            .Include(c => c.CartItems)
            .Include(c => c.OrderItems)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

        if (composition == null || composition.Source != CompositionSource.Custom || composition.AuthorId != authorId)
            throw new ApiException(404, "not_found", "Состав не найден");

        if (composition.CartItems.Count != 0 || composition.OrderItems.Count != 0)
            throw new ApiException(409, "composition_in_use", "Нельзя редактировать состав, который уже в корзине или в заказе");

        var flowerIds = request.Items.Select(i => i.FlowerId).ToList();
        if (flowerIds.Count != flowerIds.Distinct().Count())
            throw new ApiException(400, "validation_failed", "Цветы в букете не должны повторяться");

        var flowers = await _db.Flowers
            .AsNoTracking()
            .Where(f => flowerIds.Contains(f.Id))
            .ToDictionaryAsync(f => f.Id, ct);

        foreach (var item in request.Items)
        {
            if (!flowers.TryGetValue(item.FlowerId, out var flower))
                throw new ApiException(400, "validation_failed", $"Цветок {item.FlowerId} не найден");
            if (!flower.IsActive)
                throw new ApiException(409, "flower_inactive", $"Цветок {item.FlowerId} снят с продажи");
        }

        // что происходит...
        _db.CompositionItems.RemoveRange(composition.Items);
        composition.Items.Clear();

        decimal flowersTotal = 0;
        foreach (var item in request.Items)
        {
            var flower = flowers[item.FlowerId];
            flowersTotal += item.Qty * flower.PricePerStem;
            composition.Items.Add(new CompositionItem { FlowerId = flower.Id, Qty = item.Qty });
        }

        composition.CardText = request.CardText;
        composition.PackagingId = request.PackagingId;
        composition.RibbonId = request.RibbonId;

        decimal packagingPrice = 0;
        if (request.PackagingId.HasValue)
        {
            var packaging = await _db.Packagings.FirstOrDefaultAsync(p => p.Id == request.PackagingId, ct)
                ?? throw new ApiException(400, "validation_failed", "Упаковка не найдена");
            packagingPrice = packaging.Price;
        }

        composition.Price = flowersTotal + packagingPrice;

        await _db.SaveChangesAsync(ct);

        return await GetByIdAsync(composition.Id, authorId, ct)
            ?? throw new Exception("Состав не найден после обновления");
    }

    /// <summary>Выполняет GetUserCompositionsAsync.</summary>
    /// <exception cref="Florarium.Api.Exceptions.ApiException">Возникает при нарушении бизнес-правил операции.</exception>
    public async Task<PagedResponse<CompositionListDto>> GetUserCompositionsAsync(Guid userId, int page, int pageSize, CancellationToken ct)
    {
        var query = _db.Compositions
            .AsNoTracking()
            .Where(c => c.AuthorId == userId && c.Source == CompositionSource.Custom)
            .OrderByDescending(c => c.CreatedAt);

        var total = await query.CountAsync(ct);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CompositionListDto
            {
                Id = c.Id,
                Title = c.Title,
                PhotoUrl = c.PhotoUrl,
                Price = c.Price,
                CreatedAt = c.CreatedAt,
                FlowersPreview = c.Items.Select(i => i.Flower.Name + " " + i.Qty).ToList(),
                OrdersCount = c.OrderItems.Count,
                IsEditable = !c.CartItems.Any() && !c.OrderItems.Any(),
                Availability = new CompositionAvailabilityDto
                {
                    MaxQty = c.Items.Any(i => !i.Flower.IsActive) ? 0 : c.Items.Min(i => (i.Flower.Stock.Available - i.Flower.Stock.Reserved) / i.Qty),
                    InStock = !c.Items.Any(i => !i.Flower.IsActive) && c.Items.Min(i => (i.Flower.Stock.Available - i.Flower.Stock.Reserved) / i.Qty) > 0
                }
            })
            .ToListAsync(ct);

        return new PagedResponse<CompositionListDto> { Items = items, Total = total, Page = page, PageSize = pageSize };
    }

    /// <summary>Выполняет DeleteCompositionAsync.</summary>
    /// <exception cref="Florarium.Api.Exceptions.ApiException">Возникает при нарушении бизнес-правил операции.</exception>
    public async Task DeleteCompositionAsync(Guid userId, Guid id, CancellationToken ct)
    {
        var comp = await _db.Compositions
            .Include(c => c.CartItems)
            .Include(c => c.OrderItems)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

        if (comp == null || comp.Source != CompositionSource.Custom || comp.AuthorId != userId)
            throw new ApiException(404, "not_found", "Состав не найден");

        if (comp.CartItems.Count != 0 || comp.OrderItems.Count != 0)
            throw new ApiException(409, "composition_in_use", "Нельзя удалить состав, который используется в корзине или заказах");

        _db.Compositions.Remove(comp);
        await _db.SaveChangesAsync(ct);
    }
}
