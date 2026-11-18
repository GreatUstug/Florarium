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
/// <summary>Отзывы покупателей на купленные позиции заказов.</summary>
public class RatingsController(RatingsService ratingsService) : ControllerBase
{
    private readonly RatingsService _ratingsService = ratingsService;

    [HttpPost("{id:guid}/rating")]
    /// <summary>Добавляет отзыв на доставленную позицию заказа.</summary>
    /// <remarks>Заказ должен быть доставлен, позиция не должна иметь предыдущий отзыв.</remarks>
    /// <param name="id">Идентификатор позиции заказа.</param>
    /// <param name="request">Оценка, теги и текстовый отзыв.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Созданный отзыв.</returns>
    /// <response code="201">Отзыв создан.</response>
    /// <response code="400">Оценка, теги или текст не прошли валидацию.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="404">Позиция заказа не найдена.</response>
    /// <response code="409">Заказ не доставлен или отзыв уже оставлен.</response>
    [ProducesResponseType(typeof(RatingDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
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