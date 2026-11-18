using System.Security.Claims;

using Florarium.Api.Dtos.Cart;
using Florarium.Api.Exceptions;
using Florarium.Api.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Florarium.Api.Controllers;

/// <summary>Управление корзиной авторизованного пользователя.</summary>
/// <remarks>Все методы требуют cookie-аутентификации.</remarks>
[ApiController]
[Route("api/cart")]
[Authorize]
public class CartController(CartService cartService) : ControllerBase
{
    private readonly CartService _cartService = cartService;

    /// <summary>Возвращает текущее содержимое корзины.</summary>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Позиции корзины, итоговую сумму и срок резерва.</returns>
    /// <response code="200">Корзина получена.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    [HttpGet]
    [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CartResponse>> Get(CancellationToken ct)
    {
        return Ok(await _cartService.GetCartAsync(GetUserId(), ct));
    }

    /// <summary>Добавляет состав букета в корзину.</summary>
    /// <param name="request">Идентификатор состава и количество.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Обновленная корзина.</returns>
    /// <response code="200">Позиция добавлена.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="404">Состав не найден.</response>
    /// <response code="409">Состав снят с продажи или недостаточно товара на складе.</response>
    [HttpPost("items")]
    [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CartResponse>> AddItem([FromBody] AddToCartRequest request, CancellationToken ct)
    {
        return Ok(await _cartService.AddItemAsync(GetUserId(), request, ct));
    }

    /// <summary>Изменяет количество позиции корзины.</summary>
    /// <param name="id">Идентификатор позиции корзины.</param>
    /// <param name="request">Новое количество.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Обновленная корзина.</returns>
    /// <response code="200">Количество изменено.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="404">Позиция не найдена.</response>
    /// <response code="409">Недостаточно остатка или состав недоступен.</response>
    [HttpPut("items/{id:guid}")]
    [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CartResponse>> UpdateQty(Guid id, [FromBody] UpdateCartItemRequest request, CancellationToken ct)
    {
        return Ok(await _cartService.UpdateItemQtyAsync(GetUserId(), id, request.Qty, ct));
    }

    /// <summary>Удаляет одну позицию из корзины.</summary>
    /// <param name="id">Идентификатор позиции корзины.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Корзина после удаления позиции.</returns>
    /// <response code="200">Позиция удалена.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="404">Позиция не найдена.</response>
    [HttpDelete("items/{id:guid}")]
    [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CartResponse>> RemoveItem(Guid id, CancellationToken ct)
    {
        return Ok(await _cartService.RemoveItemAsync(GetUserId(), id, ct));
    }

    /// <summary>Полностью очищает корзину.</summary>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Пустой ответ.</returns>
    /// <response code="204">Корзина очищена.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Clear(CancellationToken ct)
    {
        await _cartService.ClearCartAsync(GetUserId(), ct);
        return NoContent();
    }

    private Guid GetUserId()
    {
        var str = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(str, out var id)) throw new UnauthorizedAccessException();
        return id;
    }

    private ActionResult HandleApiException(ApiException ex)
    {
        return StatusCode(ex.StatusCode, new ProblemDetails
        {
            Title = ex.Message,
            Status = ex.StatusCode,
            Extensions = { ["code"] = ex.Code }
        });
    }
}