using System.Security.Claims;
using Florarium.Api.Dtos.Cart;
using Florarium.Api.Exceptions;
using Florarium.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Florarium.Api.Controllers;

[ApiController]
[Route("api/cart")]
[Authorize]
public class CartController(CartService cartService) : ControllerBase
{
    private readonly CartService _cartService = cartService;

    [HttpGet]
    public async Task<ActionResult<CartResponse>> Get(CancellationToken ct)
    {
        return Ok(await _cartService.GetCartAsync(GetUserId(), ct));
    }

    [HttpPost("items")]
    public async Task<ActionResult<CartResponse>> AddItem([FromBody] AddToCartRequest request, CancellationToken ct)
    {
        try
        {
            return Ok(await _cartService.AddItemAsync(GetUserId(), request, ct));
        }
        catch (ApiException ex)
        {
            return HandleApiException(ex);
        }
    }

    [HttpPut("items/{id:guid}")]
    public async Task<ActionResult<CartResponse>> UpdateQty(Guid id, [FromBody] UpdateCartItemRequest request, CancellationToken ct)
    {
        try
        {
            return Ok(await _cartService.UpdateItemQtyAsync(GetUserId(), id, request.Qty, ct));
        }
        catch (ApiException ex)
        {
            return HandleApiException(ex);
        }
    }

    [HttpDelete("items/{id:guid}")]
    public async Task<ActionResult<CartResponse>> RemoveItem(Guid id, CancellationToken ct)
    {
        try
        {
            return Ok(await _cartService.RemoveItemAsync(GetUserId(), id, ct));
        }
        catch (ApiException ex)
        {
            return HandleApiException(ex);
        }
    }

    [HttpDelete]
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