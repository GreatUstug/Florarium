namespace Florarium.Api.Dtos.Flowers;

/// <summary>Модель данных FlowerListItemDto.</summary>
public class FlowerListItemDto
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
    /// <summary>Свойство LatinName.</summary>
    /// <example>пример</example>
    public string? LatinName { get; set; }
    /// <summary>Свойство ColorFamily.</summary>
    /// <example>пример</example>
    public string? ColorFamily { get; set; }
    /// <summary>Свойство PhotoUrl.</summary>
    /// <example>пример</example>
    public string? PhotoUrl { get; set; }
    /// <summary>Свойство PricePerStem.</summary>
    /// <example>100.00</example>
    public decimal PricePerStem { get; set; }
    /// <summary>Свойство Free.</summary>
    /// <example>1</example>
    public int Free { get; set; }
    /// <summary>Свойство BackInStockAt.</summary>
    /// <example>2026-11-20</example>
    public DateOnly? BackInStockAt { get; set; }
}
