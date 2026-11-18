using System.ComponentModel.DataAnnotations;

namespace Florarium.Api.Dtos.Compositions;

/// <summary>Данные для создания или обновления индивидуального состава букета.</summary>
public class SaveCompositionRequest
{
    /// <summary>Позиции состава с идентификаторами цветов и количеством.</summary>
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

    /// <summary>Текст открытки до 500 символов.</summary>
    /// <example>С днем рождения!</example>
    [MaxLength(500)]
    /// <summary>Свойство CardText.</summary>
    /// <example>пример</example>
    public string? CardText { get; set; }
}
