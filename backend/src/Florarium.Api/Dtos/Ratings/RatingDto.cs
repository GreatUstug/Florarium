namespace Florarium.Api.Dtos.Ratings;

public class RatingDto
{
    public Guid Id { get; set; }
    public short Stars { get; set; }
    public string[]? Tags { get; set; }
    public string? Text { get; set; }
    public string AuthorName { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}