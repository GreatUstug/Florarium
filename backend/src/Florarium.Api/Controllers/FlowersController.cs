using Florarium.Api.Dtos.Flowers;
using Florarium.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Florarium.Api.Controllers;

[ApiController]
[Route("api/flowers")]
public class FlowersController : ControllerBase
{
    private readonly FlowersService _flowers;

    public FlowersController(FlowersService flowers) => _flowers = flowers;

    /// <summary>
    /// Список цветов для конструктора и витрины.
    /// Возвращает только активные цветы (IsActive = true).
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<FlowerListItemDto>>> GetList(
        [FromQuery] string? search,
        [FromQuery] string[]? color,
        [FromQuery] bool onlyAvailable = false,
        CancellationToken ct = default)
    {
        var list = await _flowers.GetListAsync(search, color, onlyAvailable, ct);
        return Ok(list);
    }

    /// <summary>
    /// Уникальные значения ColorFamily среди активных цветов.
    /// Используется для фильтра на экране каталога.
    /// </summary>
    [HttpGet("colors")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetColors(CancellationToken ct = default)
    {
        var colors = await _flowers.GetColorsAsync(ct);
        return Ok(colors);
    }
}