using Florarium.Api.Dtos.Bouquets;

namespace Florarium.Api.Dtos.Compositions;

/// <summary>Модель данных CompositionDetailDto.</summary>
public class CompositionDetailDto
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

    /// <summary>Свойство CardText.</summary>
    /// <example>пример</example>
    public string? CardText { get; set; }
    /// <summary>Свойство IsEditable.</summary>
    /// <example>true</example>
    public bool IsEditable { get; set; }
}

