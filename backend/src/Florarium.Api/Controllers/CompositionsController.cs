using System.Security.Claims;

using Florarium.Api.Dtos.Compositions;
using Florarium.Api.Exceptions;
using Florarium.Api.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Florarium.Api.Controllers;

[ApiController]
[Route("api/compositions")]
/// <summary>Расчет и управление индивидуальными составами букетов.</summary>
public class CompositionsController(CompositionsService service) : ControllerBase
{
    private readonly CompositionsService _service = service;

    [HttpPost("quote")]
    /// <summary>Рассчитывает стоимость состава и проверяет доступность остатков.</summary>
    /// <param name="request">Позиции цветов и выбранные дополнительные элементы.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Расчет стоимости и предупреждения по остаткам.</returns>
    /// <response code="200">Расчет выполнен.</response>
    /// <response code="400">Состав содержит недопустимые или неизвестные данные.</response>
    [ProducesResponseType(typeof(QuoteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<QuoteResponse>> Quote(
        [FromBody] QuoteRequest request,
        CancellationToken ct)
    {
        try
        {
            var response = await _service.QuoteAsync(request, ct);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails { Title = ex.Message, Status = 400 });
        }
    }

    [Authorize]
    [HttpPost]
    /// <summary>Создает индивидуальный состав букета.</summary>
    /// <remarks>Для создания требуется авторизация. Состав сохраняется в профиле пользователя.</remarks>
    /// <param name="request">Данные нового состава.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Созданный состав.</returns>
    /// <response code="201">Состав создан.</response>
    /// <response code="400">Данные состава некорректны.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="409">Цветок снят с продажи или упаковка недоступна.</response>
    [ProducesResponseType(typeof(CompositionDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CompositionDetailDto>> Create([FromBody] SaveCompositionRequest request, CancellationToken ct)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        try
        {
            var result = await _service.CreateAsync(request, userId, ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (ApiException ex)
        {
            return StatusCode(ex.StatusCode, new ProblemDetails
            {
                Title = ex.Message,
                Status = ex.StatusCode,
                Extensions = { ["code"] = ex.Code }
            });
        }
    }

    [HttpGet("{id:guid}")]
    /// <summary>Возвращает состав букета по идентификатору.</summary>
    /// <param name="id">Идентификатор состава.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Подробная карточка состава.</returns>
    /// <response code="200">Состав найден.</response>
    /// <response code="404">Состав не найден.</response>
    [ProducesResponseType(typeof(CompositionDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CompositionDetailDto>> GetById(Guid id, CancellationToken ct)
    {
        Guid? userId = null;
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userIdStr, out var parsedId)) userId = parsedId;

        var result = await _service.GetByIdAsync(id, userId, ct);
        return result == null ? NotFound() : Ok(result);
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    /// <summary>Изменяет пользовательский состав букета.</summary>
    /// <remarks>Редактирование запрещено, если состав уже находится в корзине или заказе.</remarks>
    /// <param name="id">Идентификатор состава.</param>
    /// <param name="request">Новые данные состава.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Обновленный состав.</returns>
    /// <response code="200">Состав обновлен.</response>
    /// <response code="400">Данные состава некорректны.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="404">Состав не найден.</response>
    /// <response code="409">Состав используется или содержит недоступный товар.</response>
    [ProducesResponseType(typeof(CompositionDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CompositionDetailDto>> Update(Guid id, [FromBody] SaveCompositionRequest request, CancellationToken ct)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        try
        {
            var result = await _service.UpdateAsync(id, request, userId, ct);
            return Ok(result);
        }
        catch (ApiException ex)
        {
            return StatusCode(ex.StatusCode, new ProblemDetails
            {
                Title = ex.Message,
                Status = ex.StatusCode,
                Extensions = { ["code"] = ex.Code }
            });
        }
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    /// <summary>Удаляет пользовательский состав букета.</summary>
    /// <remarks>Удаление запрещено, если состав используется в корзине или заказах.</remarks>
    /// <param name="id">Идентификатор состава.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Пустой ответ.</returns>
    /// <response code="204">Состав удален.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="404">Состав не найден.</response>
    /// <response code="409">Состав используется в корзине или заказах.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        try
        {
            await _service.DeleteCompositionAsync(userId, id, ct);
            return NoContent();
        }
        catch (ApiException ex)
        {
            return StatusCode(ex.StatusCode, new ProblemDetails 
            { 
                Title = ex.Message, 
                Status = ex.StatusCode, 
                Extensions = { ["code"] = ex.Code } 
            });
        }
    }
}