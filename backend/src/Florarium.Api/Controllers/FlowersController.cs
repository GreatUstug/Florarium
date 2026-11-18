using Florarium.Api.Dtos.Flowers;
using Florarium.Api.Services;

using Microsoft.AspNetCore.Mvc;

namespace Florarium.Api.Controllers;

[ApiController]
[Route("api/flowers")]
/// <summary>Каталог цветов и справочник цветовых групп.</summary>
public class FlowersController : ControllerBase
{
    private readonly FlowersService _flowers;

    public FlowersController(FlowersService flowers) => _flowers = flowers;

    /// <summary>
    /// Список цветов для конструктора и витрины.
    /// Возвращает только активные цветы (IsActive = true).
    /// </summary>
    [HttpGet]
    /// <remarks>Возвращаются только активные цветы с учетом остатка при фильтре доступности.</remarks>
    /// <param name="search">Поиск по названию или характеристикам.</param>
    /// <param name="color">Фильтр по цветовым группам.</param>
    /// <param name="onlyAvailable">Показывать только цветы с остатком.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Список активных цветов.</returns>
    /// <response code="200">Список цветов получен.</response>
    [ProducesResponseType(typeof(IReadOnlyList<FlowerListItemDto>), StatusCodes.Status200OK)]
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
    /// <remarks>Возвращаются уникальные значения для активных цветов.</remarks>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Список названий цветовых групп.</returns>
    /// <response code="200">Справочник цветов получен.</response>
    [ProducesResponseType(typeof(IReadOnlyList<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<string>>> GetColors(CancellationToken ct = default)
    {
        var colors = await _flowers.GetColorsAsync(ct);
        return Ok(colors);
    }
}