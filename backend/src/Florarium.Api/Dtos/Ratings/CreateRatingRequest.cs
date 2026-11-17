using System.ComponentModel.DataAnnotations;

namespace Florarium.Api.Dtos.Ratings;

public class CreateRatingRequest
{
    [Range(1, 5)]
    public short Stars { get; set; }
    
    [MaxLength(5)]
    public string[]? Tags { get; set; }
    
    [MaxLength(2000)]
    public string? Text { get; set; }
}