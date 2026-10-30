namespace Florarium.Api.Dtos.Bouquets;

public class BouquetListItemDto
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? PhotoUrl { get; set; }
    public decimal Price { get; set; }
    public List<string> Colors { get; set; } = [];
    public BouquetAvailabilityDto Availability { get; set; } = new();
    public BouquetRatingDto Rating { get; set; } = new();
}

public class BouquetAvailabilityDto
{
    public bool InStock { get; set; }
    public int MaxQty { get; set; }
}

public class BouquetRatingDto
{
    public double? Avg { get; set; }
    public int Count { get; set; }
}