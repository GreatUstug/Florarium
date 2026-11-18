namespace Florarium.Api.Dtos.Dictionaries;

/// <summary>Модель данных RibbonDto.</summary>
public class RibbonDto
{
    /// <summary>Свойство Id.</summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid Id { get; set; }
    /// <summary>Свойство Title.</summary>
    /// <example>пример</example>
    public string Title { get; set; } = null!;
    /// <summary>Свойство ColorHex.</summary>
    /// <example>пример</example>
    public string? ColorHex { get; set; }
}
