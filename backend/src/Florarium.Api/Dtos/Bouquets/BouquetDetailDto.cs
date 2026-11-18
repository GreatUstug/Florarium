

namespace Florarium.Api.Dtos.Bouquets;

/// <summary>Модель данных BouquetDetailDto.</summary>
public class BouquetDetailDto
{
    /// <summary>Свойство Id.</summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid Id { get; set; }
    /// <summary>Свойство Title.</summary>
    /// <example>пример</example>
    public string? Title { get; set; }
    /// <summary>Свойство PhotoUrl.</summary>
    /// <example>пример</example>
    public string? PhotoUrl { get; set; }
    /// <summary>Свойство Source.</summary>
    /// <example>пример</example>
    public string Source { get; set; } = null!;
    /// <summary>Свойство Price.</summary>
    /// <example>100.00</example>
    public decimal Price { get; set; }
    /// <summary>Свойство PriceBreakdown.</summary>
    /// <example>пример</example>
    public PriceBreakdownDto PriceBreakdown { get; set; } = new();
    /// <summary>Свойство Items.</summary>
    /// <example>[]</example>
    public List<BouquetItemDto> Items { get; set; } = [];
    /// <summary>Свойство Packaging.</summary>
    /// <example>пример</example>
    public PackagingDto? Packaging { get; set; }
    /// <summary>Свойство Ribbon.</summary>
    /// <example>пример</example>
    public RibbonDto? Ribbon { get; set; }
    /// <summary>Свойство Availability.</summary>
    /// <example>пример</example>
    public BouquetDetailAvailabilityDto Availability { get; set; } = new();
    /// <summary>Свойство Rating.</summary>
    /// <example>пример</example>
    public BouquetRatingDto Rating { get; set; } = new();
}

/// <summary>Модель данных PriceBreakdownDto.</summary>
public class PriceBreakdownDto
{
    /// <summary>Свойство Flowers.</summary>
    /// <example>100.00</example>
    public decimal Flowers {get; set;}
    /// <summary>Свойство Packaging.</summary>
    /// <example>100.00</example>
    public decimal Packaging {get; set;}
}

/// <summary>Модель данных BouquetItemDto.</summary>
public class BouquetItemDto
{
    /// <summary>Свойство Flower.</summary>
    /// <example>пример</example>
    public FlowerDto Flower {get; set;} = null!;
    /// <summary>Свойство Qty.</summary>
    /// <example>1</example>
    public int Qty {get; set;}
    /// <summary>Свойство LineTotal.</summary>
    /// <example>100.00</example>
    public decimal LineTotal {get; set;}
}

/// <summary>Модель данных FlowerDto.</summary>
public class FlowerDto
{
    /// <summary>Свойство Id.</summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid Id { get; set; }
    /// <summary>Свойство Name.</summary>
    /// <example>пример</example>
    public string Name { get; set; } = null!;
    /// <summary>Свойство Variety.</summary>
    /// <example>пример</example>
    public string? Variety { get; set; }
    /// <summary>Свойство ColorFamily.</summary>
    /// <example>пример</example>
    public string? ColorFamily { get; set; }
    /// <summary>Свойство PhotoUrl.</summary>
    /// <example>пример</example>
    public string? PhotoUrl { get; set; }
    /// <summary>Свойство PricePerStem.</summary>
    /// <example>100.00</example>
    public decimal PricePerStem { get; set; }
}

/// <summary>Модель данных PackagingDto.</summary>
public class PackagingDto
{
    /// <summary>Свойство Id.</summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid Id {get; set;}
    /// <summary>Свойство Title.</summary>
    /// <example>пример</example>
    public string Title {get; set;} = null!;
    /// <summary>Свойство Price.</summary>
    /// <example>100.00</example>
    public decimal Price {get; set;}
}

/// <summary>Модель данных RibbonDto.</summary>
public class RibbonDto
{
    /// <summary>Свойство Id.</summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid Id {get; set;}
    /// <summary>Свойство Title.</summary>
    /// <example>пример</example>
    public string Title {get; set;} = null!;
    /// <summary>Свойство ColorHex.</summary>
    /// <example>пример</example>
    public string? ColorHex {get; set;}
}

/// <summary>Модель данных BouquetDetailAvailabilityDto.</summary>
public class BouquetDetailAvailabilityDto
{
    /// <summary>Свойство InStock.</summary>
    /// <example>true</example>
    public bool InStock {get; set;}
    /// <summary>Свойство MaxQty.</summary>
    /// <example>1</example>
    public int MaxQty {get; set;}
    /// <summary>Свойство BackInStockAt.</summary>
    /// <example>2026-11-20</example>
    public DateOnly? BackInStockAt {get; set;}
}
