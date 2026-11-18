using Florarium.Api.Dtos.Flowers;
using Florarium.Data;
using Microsoft.EntityFrameworkCore;

namespace Florarium.Api.Services;

/// <summary>Сервис FlowersService.</summary>
public class FlowersService(AppDbContext db)
{
    private readonly AppDbContext _db = db;

    /// <summary>Выполняет GetListAsync.</summary>
    /// <exception cref="Florarium.Api.Exceptions.ApiException">Возникает при нарушении бизнес-правил операции.</exception>
    public async Task<IReadOnlyList<FlowerListItemDto>> GetListAsync(string? search, IEnumerable<string>? colors, bool onlyAvailible, CancellationToken ct)
    {
        var q = _db.Flowers
            .AsNoTracking()
            .Include(f => f.Stock)
            .Where(f => f.IsActive)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search.Trim()}%";
            q = q.Where(f =>
                EF.Functions.ILike(f.Name, pattern) ||
                (f.Variety != null && EF.Functions.ILike(f.Variety, pattern))
            );
        }

        if (colors is not null)
        {
            var colorsArray = colors.Where(c => !string.IsNullOrWhiteSpace(c)).ToArray();
            if (colorsArray.Length > 0)
            {
                q = q.Where(f => f.ColorFamily != null && colorsArray.Contains(f.ColorFamily));
            }
        }

        if (onlyAvailible)
        {
            q = q.Where(f => f.Stock.Available - f.Stock.Reserved > 0);
        }

        var list = await q
            .OrderBy(f => f.Name)
            .Select(f => new FlowerListItemDto
            {
                Id = f.Id,
                Name = f.Name,
                Variety = f.Variety,
                LatinName = f.LatinName,
                ColorFamily = f.ColorFamily,
                PhotoUrl = f.PhotoUrl,
                PricePerStem = f.PricePerStem,
                Free = f.Stock.Available - f.Stock.Reserved,
                BackInStockAt = f.Stock.BackInStockAt
            })
            .ToListAsync(ct);

        return list;
    }

    /// <summary>Выполняет GetColorsAsync.</summary>
    /// <exception cref="Florarium.Api.Exceptions.ApiException">Возникает при нарушении бизнес-правил операции.</exception>
    public async Task<IReadOnlyList<string>> GetColorsAsync(CancellationToken ct)
    {
        return await _db.Flowers
            .AsNoTracking()
            .Where(f => f.IsActive && f.ColorFamily != null)
            .Select(f => f.ColorFamily!)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();
    }
}
