namespace Florarium.Api.Dtos.Dictionaries;

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
