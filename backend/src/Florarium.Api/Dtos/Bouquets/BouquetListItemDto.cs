namespace Florarium.Api.Dtos.Bouquets;

/// <summary>Модель данных BouquetListItemDto.</summary>
public class BouquetListItemDto
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
    /// <summary>Свойство Price.</summary>
    /// <example>100.00</example>
    public decimal Price { get; set; }
    /// <summary>Свойство Colors.</summary>
    /// <example>[]</example>
    public List<string> Colors { get; set; } = [];
    /// <summary>Свойство Availability.</summary>
    /// <example>пример</example>
    public BouquetAvailabilityDto Availability { get; set; } = new();
    /// <summary>Свойство Rating.</summary>
    /// <example>пример</example>
    public BouquetRatingDto Rating { get; set; } = new();
}

/// <summary>Модель данных BouquetAvailabilityDto.</summary>
public class BouquetAvailabilityDto
{
    /// <summary>Свойство InStock.</summary>
    /// <example>true</example>
    public bool InStock { get; set; }
    /// <summary>Свойство MaxQty.</summary>
    /// <example>1</example>
    public int MaxQty { get; set; }
}

/// <summary>Модель данных BouquetRatingDto.</summary>
public class BouquetRatingDto
{
    /// <summary>Свойство Avg.</summary>
    /// <example>100.00</example>
    public double? Avg { get; set; }
    /// <summary>Свойство Count.</summary>
    /// <example>1</example>
    public int Count { get; set; }
}
