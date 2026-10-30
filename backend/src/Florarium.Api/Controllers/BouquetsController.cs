using Florarium.Api.Dtos.Bouquets;

using Microsoft.AspNetCore.Mvc;

public class BouquetsController(BouquetsService bouquets) : ControllerBase
{
    private readonly BouquetsService _bouquets = bouquets;

    [HttpGet]
    public async Task<IActionResult> GetCatalog(
        [FromQuery] string[]? color,
        [FromQuery] decimal? priceFrom,
        [FromQuery] decimal? priceTo,
        [FromQuery] bool inStock = false,
        [FromQuery] string sort = "popular",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12,
        CancellationToken ct = default)
    {
        var result = await _bouquets.GetCatalogAsync(color, priceFrom, priceTo, inStock, sort, page, pageSize, ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BouquetDetailDto>> GetById(Guid id, CancellationToken ct)
    {
        var result = await _bouquets.GetByIdAsync(id, ct);
        return result == null ? NotFound() : Ok(result);
    }
}