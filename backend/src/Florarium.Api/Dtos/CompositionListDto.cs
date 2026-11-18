namespace Florarium.Api.Dtos.Compositions;

/// <summary>Модель данных CompositionListDto.</summary>
public class CompositionListDto
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
    /// <summary>Свойство FlowersPreview.</summary>
    /// <example>[]</example>
    public List<string> FlowersPreview { get; set; } = [];
    /// <summary>Свойство CreatedAt.</summary>
    /// <example>2026-11-18T12:00:00Z</example>
    public DateTime CreatedAt { get; set; }
    /// <summary>Свойство Availability.</summary>
    /// <example>пример</example>
    public CompositionAvailabilityDto Availability { get; set; } = new();
    /// <summary>Свойство OrdersCount.</summary>
    /// <example>1</example>
    public int OrdersCount { get; set; }
    /// <summary>Свойство IsEditable.</summary>
    /// <example>true</example>
    public bool IsEditable { get; set; }
}

/// <summary>Модель данных CompositionAvailabilityDto.</summary>
public class CompositionAvailabilityDto
{
    /// <summary>Свойство InStock.</summary>
    /// <example>true</example>
    public bool InStock { get; set; }
    /// <summary>Свойство MaxQty.</summary>
    /// <example>1</example>
    public int MaxQty { get; set; }
}
