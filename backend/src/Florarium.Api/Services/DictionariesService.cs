using Florarium.Api.Dtos.Dictionaries;
using Florarium.Data;
using Microsoft.EntityFrameworkCore;

namespace Florarium.Api.Services;

public class DictionariesService(AppDbContext db)
{
    private readonly AppDbContext _db = db;

    public async Task<IReadOnlyList<PackagingDto>> GetPackagingsAsync(CancellationToken ct)
    {
        return await _db.Packagings
            .AsNoTracking()
            .OrderBy(p => p.Price)
            .Select(p => new PackagingDto { Id = p.Id, Title = p.Title, Price = p.Price })
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<RibbonDto>> GetRibbonsAsync(CancellationToken ct)
    {
        return await _db.Ribbons
            .AsNoTracking()
            .OrderBy(r => r.Title)
            .Select(r => new RibbonDto { Id = r.Id, Title = r.Title, ColorHex = r.ColorHex })
            .ToListAsync(ct);
    }
}