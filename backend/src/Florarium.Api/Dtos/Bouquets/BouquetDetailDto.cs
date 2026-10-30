

namespace Florarium.Api.Dtos.Bouquets;

public class BouquetDetailDto
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
}

public class PriceBreakdownDto
{
    public decimal Flowers {get; set;}
    public decimal Packaging {get; set;}
}

public class BouquetItemDto
{
    public FlowerDto Flower {get; set;} = null!;
    public int Qty {get; set;}
    public decimal LineTotal {get; set;}
}

public class FlowerDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Variety { get; set; }
    public string? ColorFamily { get; set; }
    public string? PhotoUrl { get; set; }
    public decimal PricePerStem { get; set; }
}

public class PackagingDto
{
    public Guid Id {get; set;}
    public string Title {get; set;} = null!;
    public decimal Price {get; set;}
}

public class RibbonDto
{
    public Guid Id {get; set;}
    public string Title {get; set;} = null!;
    public string? ColorHex {get; set;}
}

public class BouquetDetailAvailabilityDto
{
    public bool InStock {get; set;}
    public int MaxQty {get; set;}
    public DateOnly? BackInStockAt {get; set;}
}