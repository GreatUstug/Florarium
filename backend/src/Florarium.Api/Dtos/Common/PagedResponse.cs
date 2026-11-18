namespace Florarium.Api.Dtos.Common;

/// <summary>Модель данных PagedResponse.</summary>
public class PagedResponse<T>
{
    /// <summary>Свойство Items.</summary>
    /// <example>[]</example>
    public IEnumerable<T> Items {get; set;} = [];
    /// <summary>Свойство Total.</summary>
    /// <example>1</example>
    public int Total {get; set;}
    /// <summary>Свойство Page.</summary>
    /// <example>1</example>
    public int Page {get; set;}
    /// <summary>Свойство PageSize.</summary>
    /// <example>1</example>
    public int PageSize {get; set;}
}
