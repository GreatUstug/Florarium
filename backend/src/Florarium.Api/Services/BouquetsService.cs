using System.Collections.Immutable;
using System.Drawing;

using Florarium.Api.Dtos.Bouquets;
using Florarium.Data;
using Florarium.Domain.Entities;
using Florarium.Domain.Enums;

using Microsoft.EntityFrameworkCore;

public class BouquetsService(AppDbContext db)
{
    private readonly AppDbContext _db = db;

    public async Task<object> GetCatalogAsync(
        string[]? colors, decimal? priceFrom, decimal? priceTo,
        bool inStock, string sort, int page, int pageSize, CancellationToken ct)
    {
        // сначала запрашиваем только шаблоны букетов
        var query = _db.Compositions
            .AsNoTracking()
            .Where(c => c.Source == CompositionSource.Template);

        // фильтры
        if (priceFrom.HasValue) query = query.Where(c => c.Price >= priceFrom.Value);
        if (priceTo.HasValue) query = query.Where(c => c.Price <= priceTo.Value);

        if (colors != null && colors.Length > 0)
        {
            // букет подходит если хотя бы один цветок в нем нужного цвета
            query = query.Where(c => c.Items.Any(i =>
                i.Flower.ColorFamily != null && colors.Contains(i.Flower.ColorFamily)));
        }

        // считаем total до пагинации
        var total = await query.CountAsync(ct);

        // сортируем
        query = sort.ToLower() switch
        {
            "priceasc" => query.OrderBy(c => c.Price),
            "pricedesc" => query.OrderByDescending(c => c.Price),
            "new" => query.OrderByDescending(c => c.CreatedAt),
            "popular" => query.OrderByDescending(c => c.OrderItems.Sum(oi => oi.Qty)),

            _ => query.OrderByDescending(c => c.CreatedAt)
        };

        // делаем проекцию в DTO
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new BouquetListItemDto
            {
                Id = c.Id,
                Title = c.Title,
                PhotoUrl = c.PhotoUrl,
                Price = c.Price,
                Colors = c.Items.Select(i => i.Flower.ColorFamily).Where(col => col != null).Distinct().ToList(),


                // Считаем maxQty: берем минимальное значение (Available - Reserved) / Qty
                // Если цветок неактивен, ставим 0.
                // В SQL это превратится в MIN(CAST((available - reserved) AS decimal) / qty)
                Availability = c.Items.Any(i => !i.Flower.IsActive)
                ? new BouquetAvailabilityDto { MaxQty = 0, InStock = false }
                : new BouquetAvailabilityDto
                {
                    // Считаем лимит по самому дефицитному цветку
                    MaxQty = c.Items.Min(i => (i.Flower.Stock.Available - i.Flower.Stock.Reserved) / i.Qty),

                    // Если собрать можно больше 0, значит букет в наличии
                    InStock = c.Items.Min(i => (i.Flower.Stock.Available - i.Flower.Stock.Reserved) / i.Qty) > 0
                },

                Rating = new BouquetRatingDto
                {
                    Count = c.OrderItems.Count(oi => oi.Rating != null),
                    Avg = c.OrderItems
                        .Where(oi => oi.Rating != null)
                        .Average(oi => (double?)oi.Rating!.Stars)
                }
            })
            .ToListAsync(ct);

        return new { items, total, page, pageSize };
    }

    public async Task<BouquetDetailDto?> GetByIdAsync(Guid id, CancellationToken ct)
{
    return await _db.Compositions
        .AsNoTracking()
        .Where(c => c.Id == id && c.Source == CompositionSource.Template)
        .Select(c => new BouquetDetailDto
        {
            Id = c.Id,
            Title = c.Title,
            PhotoUrl = c.PhotoUrl,
            Source = c.Source.ToString().ToLower(),
            Price = c.Price,

            PriceBreakdown = new PriceBreakdownDto
            {
                Flowers = c.Items.Sum(i => i.Qty * i.Flower.PricePerStem),
                Packaging = c.Packaging != null ? c.Packaging.Price : 0
            },

            Items = c.Items.Select(i => new BouquetItemDto
            {
                Qty = i.Qty,
                LineTotal = i.Qty * i.Flower.PricePerStem,
                Flower = new FlowerDto
                {
                    Id = i.FlowerId,
                    Name = i.Flower.Name,
                    Variety = i.Flower.Variety,
                    ColorFamily = i.Flower.ColorFamily,
                    PhotoUrl = i.Flower.PhotoUrl,
                    PricePerStem = i.Flower.PricePerStem
                }
            }).ToList(),

            Packaging = c.Packaging != null ? new PackagingDto { Id = c.Packaging.Id, Title = c.Packaging.Title, Price = c.Packaging.Price } : null,
            Ribbon = c.Ribbon != null ? new RibbonDto { Id = c.Ribbon.Id, Title = c.Ribbon.Title, ColorHex = c.Ribbon.ColorHex } : null,

            Availability = c.Items.Any(i => !i.Flower.IsActive)
                ? new BouquetDetailAvailabilityDto 
                  { 
                      MaxQty = 0, 
                      InStock = false, 
                      BackInStockAt = null 
                  }
                : new BouquetDetailAvailabilityDto
                  {
                      MaxQty = c.Items.Min(i => (i.Flower.Stock.Available - i.Flower.Stock.Reserved) / i.Qty),
                      
                      InStock = c.Items.Min(i => (i.Flower.Stock.Available - i.Flower.Stock.Reserved) / i.Qty) > 0,

                      BackInStockAt = c.Items
                          .Where(i => (i.Flower.Stock.Available - i.Flower.Stock.Reserved) < i.Qty)
                          .Max(i => i.Flower.Stock.BackInStockAt)
                  },

            Rating = new BouquetRatingDto
            {
                Count = c.OrderItems.Count(oi => oi.Rating != null),
                Avg = c.OrderItems
                    .Where(oi => oi.Rating != null)
                    .Average(oi => (double?)oi.Rating!.Stars)
            }
        })
        .FirstOrDefaultAsync(ct);
    }
}