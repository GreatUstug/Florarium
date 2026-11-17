using Florarium.Api.Dtos.Ratings;
using Florarium.Api.Exceptions;
using Florarium.Data;
using Florarium.Domain.Entities;
using Florarium.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Florarium.Api.Services;

public class RatingsService(AppDbContext db)
{
    private readonly AppDbContext _db = db;

    public async Task<RatingDto> AddRatingAsync(Guid userId, Guid orderItemId, CreateRatingRequest request, CancellationToken ct)
    {
        var orderItem = await _db.OrderItems
            .Include(oi => oi.Order)
            .Include(oi => oi.Rating)
            .FirstOrDefaultAsync(oi => oi.Id == orderItemId, ct);

        if (orderItem == null || orderItem.Order.CustomerId != userId)
            throw new ApiException(404, "not_found", "Позиция заказа не найдена");

        if (orderItem.Order.Status != OrderStatus.Done)
            throw new ApiException(409, "order_not_done", "Заказ еще не доставлен");

        if (orderItem.Rating != null)
            throw new ApiException(409, "already_rated", "Вы уже оставили отзыв на этот букет");

        if (request.Tags != null)
        {
            if (request.Tags.Length > 5) throw new ApiException(400, "validation_failed", "Не более 5 тегов");
            if (request.Tags.Any(t => t.Length > 30)) throw new ApiException(400, "validation_failed", "Тег не длиннее 30 символов");
        }

        var rating = new Rating
        {
            OrderItemId = orderItemId,
            CustomerId = userId,
            Stars = request.Stars,
            Tags = request.Tags,
            Text = request.Text,
            CreatedAt = DateTime.UtcNow
        };

        _db.Ratings.Add(rating);
        await _db.SaveChangesAsync(ct);

        var user = await _db.Users.FindAsync([userId], ct);

        return new RatingDto
        {
            Id = rating.Id,
            Stars = rating.Stars,
            Tags = rating.Tags,
            Text = rating.Text,
            AuthorName = user!.Name.Split(' ')[0],
            CreatedAt = rating.CreatedAt
        };
    }
}