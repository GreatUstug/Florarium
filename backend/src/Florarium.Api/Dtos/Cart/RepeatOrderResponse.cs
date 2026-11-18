namespace Florarium.Api.Dtos.Cart;

/// <summary>Модель данных RepeatOrderResponse.</summary>
public class RepeatOrderResponse
{
    /// <summary>Свойство Cart.</summary>
    /// <example>пример</example>
    public CartResponse Cart { get; set; } = null!;
    /// <summary>Свойство Skipped.</summary>
    /// <example>[]</example>
    public List<SkippedItemDto> Skipped { get; set; } = new();
}

/// <summary>Модель данных SkippedItemDto.</summary>
public class SkippedItemDto
{
    /// <summary>Свойство CompositionId.</summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid CompositionId { get; set; }
    /// <summary>Свойство Title.</summary>
    /// <example>пример</example>
    public string? Title { get; set; }
    /// <summary>Свойство Reason.</summary>
    /// <example>пример</example>
    public string Reason { get; set; } = null!; // out_of_stock, flower_inactive
}
