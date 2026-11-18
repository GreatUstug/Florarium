namespace Florarium.Api.Dtos.Ratings;

/// <summary>Модель данных RatingDto.</summary>
public class RatingDto
{
    /// <summary>Свойство Id.</summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid Id { get; set; }
    /// <summary>Свойство Stars.</summary>
    /// <example>1</example>
    public short Stars { get; set; }
    /// <summary>Свойство Tags.</summary>
    /// <example>пример</example>
    public string[]? Tags { get; set; }
    /// <summary>Свойство Text.</summary>
    /// <example>пример</example>
    public string? Text { get; set; }
    /// <summary>Свойство AuthorName.</summary>
    /// <example>пример</example>
    public string AuthorName { get; set; } = null!;
    /// <summary>Свойство CreatedAt.</summary>
    /// <example>2026-11-18T12:00:00Z</example>
    public DateTime CreatedAt { get; set; }
}
