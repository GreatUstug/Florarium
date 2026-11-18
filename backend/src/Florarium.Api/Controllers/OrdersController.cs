using System.Security.Claims;

using Florarium.Api.Dtos.Cart;
using Florarium.Api.Dtos.Orders;
using Florarium.Api.Exceptions;
using Florarium.Api.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Florarium.Api.Controllers;

/// <summary>Создание и управление заказами авторизованного пользователя.</summary>
/// <remarks>Все методы требуют cookie-аутентификации.</remarks>
[ApiController]
[Route("api/orders")]
[Authorize]
public class OrdersController(OrdersService ordersService) : ControllerBase
{
    private readonly OrdersService _ordersService = ordersService;

    /// <summary>Создает заказ из текущей корзины.</summary>
    /// <remarks>Корзина должна быть непустой, адрес существовать, а товары иметь остатки.</remarks>
    /// <param name="request">Адрес, время доставки и способ оплаты.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Краткие данные созданного заказа.</returns>
    /// <response code="201">Заказ создан.</response>
    /// <response code="400">Адрес, параметры доставки или данные корзины некорректны.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="409">Корзина пуста, товар недоступен или недостаточно остатков.</response>
    [HttpPost]
    [ProducesResponseType(typeof(OrderCreatedResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<OrderCreatedResponse>> Create([FromBody] CreateOrderRequest request, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();

        var result = await _ordersService.CreateOrderAsync(userId, request, ct);
        return StatusCode(201, result);
    }

    /// <summary>Оформляет платеж для заказа.</summary>
    /// <param name="id">Идентификатор заказа.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Актуальные данные заказа.</returns>
    /// <response code="200">Платеж обработан или отмечен как выполненный.</response>
    /// <response code="400">Заказ не требует онлайн-оплаты.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="404">Заказ не найден.</response>
    /// <response code="409">Заказ уже оплачен.</response>
    [HttpPost("{id:guid}/payments")]
    [ProducesResponseType(typeof(OrderDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<OrderDetailDto>> Pay(Guid id, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();

        var result = await _ordersService.PayOrderAsync(userId, id, ct);
        return Ok(result);
    }

    /// <summary>Возвращает подробную информацию о заказе.</summary>
    /// <param name="id">Идентификатор заказа.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Детали заказа.</returns>
    /// <response code="200">Заказ найден.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="404">Заказ не найден или не принадлежит пользователю.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderDetailDto>> GetById(Guid id, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();

        var result = await _ordersService.GetOrderByIdAsync(userId, id, ct);
        return result == null ? NotFound() : Ok(result);
    }

    /// <summary>Отменяет заказ, если он еще не передан в доставку.</summary>
    /// <param name="id">Идентификатор заказа.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Актуальные данные отмененного заказа.</returns>
    /// <response code="200">Заказ отменен.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="404">Заказ не найден.</response>
    /// <response code="409">Заказ уже нельзя отменить.</response>
    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(typeof(OrderDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<OrderDetailDto>> Cancel(Guid id, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();

        var result = await _ordersService.CancelOrderAsync(userId, id, ct);
        return Ok(result);
    }

    /// <summary>Повторно добавляет доступные позиции заказа в корзину.</summary>
    /// <param name="id">Идентификатор исходного заказа.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Обновленная корзина и пропущенные позиции.</returns>
    /// <response code="200">Повторение обработано; недоступные позиции возвращены отдельно.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="404">Заказ не найден.</response>
    [HttpPost("{id:guid}/repeat")]
    [ProducesResponseType(typeof(RepeatOrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RepeatOrderResponse>> Repeat(Guid id, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();

        var result = await _ordersService.RepeatOrderAsync(userId, id, ct);
        return Ok(result);
    }

    private bool TryGetUserId(out Guid userId)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdStr, out userId);
    }
}