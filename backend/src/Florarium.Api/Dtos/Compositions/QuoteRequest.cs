using System.ComponentModel.DataAnnotations;

namespace Florarium.Api.Dtos.Compositions;

/// <summary>Запрос расчета индивидуального состава букета.</summary>
public class QuoteRequest
{
    /// <summary>Позиции цветов состава.</summary>
    [Required]
    [MinLength(1, ErrorMessage = "Букет не может быть пустым")]
    [MaxLength(30, ErrorMessage = "Не больше 30 позиций")]
    /// <summary>Свойство Items.</summary>
    /// <example>[]</example>
    public List<QuoteItemRequest> Items { get; set; } = [];
    
    /// <summary>Необязательный идентификатор упаковки.</summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    /// <summary>Свойство PackagingId.</summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid? PackagingId { get; set; }
    /// <summary>Необязательный идентификатор ленты.</summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    /// <summary>Свойство RibbonId.</summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid? RibbonId { get; set; }
}

/// <summary>Модель данных QuoteItemRequest.</summary>
public class QuoteItemRequest
{
    /// <summary>Идентификатор цветка.</summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    [Required]
    /// <summary>Свойство FlowerId.</summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid FlowerId { get; set; }
    
    /// <summary>Количество стеблей от 1 до 200.</summary>
    /// <example>5</example>
    [Range(1, 200, ErrorMessage = "Количество должно быть от 1 до 200")]
    /// <summary>Свойство Qty.</summary>
    /// <example>1</example>
    public int Qty { get; set; }
}
