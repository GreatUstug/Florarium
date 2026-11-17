using System.Security.Claims;

using Florarium.Api.Dtos.Orders;
using Florarium.Api.Exceptions;
using Florarium.Api.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Florarium.Api.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrdersController(OrdersService ordersService) : ControllerBase
{
    private readonly OrdersService _ordersService = ordersService;

    [HttpPost]
    public async Task<ActionResult<OrderCreatedResponse>> Create([FromBody] CreateOrderRequest request, CancellationToken ct)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        try
        {
            var result = await _ordersService.CreateOrderAsync(userId, request, ct);
            return StatusCode(201, result);
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

    [HttpPost("{id:guid}/payments")]
    public async Task<ActionResult<OrderDetailDto>> Pay(Guid id, CancellationToken ct)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        try
        {
            var result = await _ordersService.PayOrderAsync(userId, id, ct);
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

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderDetailDto>> GetById(Guid id, CancellationToken ct)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        var result = await _ordersService.GetOrderByIdAsync(userId, id, ct);
        return result == null ? NotFound() : Ok(result);
    }
}