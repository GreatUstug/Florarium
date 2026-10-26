namespace Florarium.Api.Dtos.Flowers;

public class FlowerListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Variety { get; set; }
    public string? LatinName { get; set; }
    public string? ColorFamily { get; set; }
    public string? PhotoUrl { get; set; }
    public decimal PricePerStem { get; set; }
    public int Free { get; set; }
    public DateOnly? BackInStockAt { get; set; }
}