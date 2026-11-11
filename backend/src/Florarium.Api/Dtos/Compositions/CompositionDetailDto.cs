using Florarium.Api.Dtos.Bouquets;

namespace Florarium.Api.Dtos.Compositions;

public class CompositionDetailDto
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? PhotoUrl { get; set; }
    public string Source { get; set; } = null!;
    public decimal Price { get; set; }
    public PriceBreakdownDto PriceBreakdown { get; set; } = new();
    public List<BouquetItemDto> Items { get; set; } = [];
    public PackagingDto? Packaging { get; set; }
    public RibbonDto? Ribbon { get; set; }
    public BouquetDetailAvailabilityDto Availability { get; set; } = new();
    public BouquetRatingDto Rating { get; set; } = new();

    public string? CardText { get; set; }
    public bool IsEditable { get; set; }
}
