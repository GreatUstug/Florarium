using Florarium.Api.Dtos.Compositions;
using Florarium.Data;
using Microsoft.EntityFrameworkCore;

using Florarium.Api.Dtos.Compositions;

namespace Florarium.Api.Services;

public class CompositionsService(AppDbContext db)
{
    private readonly AppDbContext _db = db;

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
}