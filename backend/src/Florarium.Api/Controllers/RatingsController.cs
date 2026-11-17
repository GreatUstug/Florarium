using System.Security.Claims;
using Florarium.Api.Dtos.Ratings;
using Florarium.Api.Exceptions;
using Florarium.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Florarium.Api.Controllers;

[ApiController]
[Route("api/order-items")]
[Authorize]
public class RatingsController(RatingsService ratingsService) : ControllerBase
{
    private readonly RatingsService _ratingsService = ratingsService;

    [HttpPost("{id:guid}/rating")]
    public async Task<ActionResult<RatingDto>> AddRating(Guid id, [FromBody] CreateRatingRequest request, CancellationToken ct)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        try
        {
            var result = await _ratingsService.AddRatingAsync(userId, id, request, ct);
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
}