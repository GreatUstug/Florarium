using System.ComponentModel.DataAnnotations;

namespace Florarium.Api.Dtos.Ratings;

/// <summary>Модель данных CreateRatingRequest.</summary>
public class CreateRatingRequest
{
    [Range(1, 5)]
    /// <summary>Свойство Stars.</summary>
    /// <example>1</example>
    public short Stars { get; set; }
    
    [MaxLength(5)]
    /// <summary>Свойство Tags.</summary>
    /// <example>пример</example>
    public string[]? Tags { get; set; }
    
    [MaxLength(2000)]
    /// <summary>Свойство Text.</summary>
    /// <example>пример</example>
    public string? Text { get; set; }
}
