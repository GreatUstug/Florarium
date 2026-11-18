using System.Security.Claims;
using Florarium.Api.Dtos.Common;
using Florarium.Api.Dtos.Orders;
using Florarium.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Florarium.Api.Controllers;

[ApiController]
[Route("api/me/orders")]
[Authorize]
/// <summary>Просмотр истории заказов текущего пользователя.</summary>
public class MeOrdersController(OrdersService ordersService) : ControllerBase
{
    private readonly OrdersService _ordersService = ordersService;

    [HttpGet]
    /// <summary>Возвращает постраничный список заказов пользователя.</summary>
    /// <param name="status">Необязательный фильтр по статусу заказа.</param>
    /// <param name="page">Номер страницы.</param>
    /// <param name="pageSize">Размер страницы.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Постраничный список заказов.</returns>
    /// <response code="200">Заказы получены.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    [ProducesResponseType(typeof(PagedResponse<OrderListDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResponse<OrderListDto>>> GetMyOrders(
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12,
        CancellationToken ct = default)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        var result = await _ordersService.GetUserOrdersAsync(userId, status, page, pageSize, ct);
        return Ok(result);
    }
}