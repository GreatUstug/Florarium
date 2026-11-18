using Florarium.Api.Dtos.Bouquets;
using Florarium.Api.Dtos.Common;
using Florarium.Api.Services;

using Microsoft.AspNetCore.Mvc;

namespace Florarium.Api.Controllers;

[ApiController]
[Route("api/bouquets")]
/// <summary>Просмотр каталога букетов и отзывов.</summary>
public class BouquetsController(BouquetsService bouquets) : ControllerBase
{
    private readonly BouquetsService _bouquets = bouquets;

    [HttpGet]
    /// <summary>Возвращает постраничный каталог букетов с фильтрами.</summary>
    /// <param name="color">Фильтр по цветовой группе.</param>
    /// <param name="priceFrom">Минимальная цена букета.</param>
    /// <param name="priceTo">Максимальная цена букета.</param>
    /// <param name="inStock">Показывать только доступные букеты.</param>
    /// <param name="sort">Способ сортировки.</param>
    /// <param name="page">Номер страницы.</param>
    /// <param name="pageSize">Размер страницы.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Страница карточек букетов.</returns>
    /// <response code="200">Каталог успешно получен.</response>
    [ProducesResponseType(typeof(PagedResponse<BouquetListItemDto>), StatusCodes.Status200OK)]
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
    /// <summary>Возвращает подробную карточку букета.</summary>
    /// <param name="id">Идентификатор букета.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Карточка букета или ответ об отсутствии.</returns>
    /// <response code="200">Букет найден.</response>
    /// <response code="404">Букет с указанным идентификатором не найден.</response>
    [ProducesResponseType(typeof(BouquetDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BouquetDetailDto>> GetById(Guid id, CancellationToken ct)
    {
        var result = await _bouquets.GetByIdAsync(id, ct);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpGet("{id:guid}/ratings")]
    /// <summary>Возвращает постраничный список отзывов букета.</summary>
    /// <param name="id">Идентификатор букета.</param>
    /// <param name="page">Номер страницы.</param>
    /// <param name="pageSize">Размер страницы.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Отзывы и агрегированные данные рейтинга.</returns>
    /// <response code="200">Отзывы успешно получены.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRatings(Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 12, CancellationToken ct = default)
    {
        var result = await _bouquets.GetRatingsAsync(id, page, pageSize, ct);
        return Ok(result);
    }
}