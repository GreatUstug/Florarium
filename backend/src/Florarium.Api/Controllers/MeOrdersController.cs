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
public class MeOrdersController(OrdersService ordersService) : ControllerBase
{
    private readonly OrdersService _ordersService = ordersService;

    [HttpGet]
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