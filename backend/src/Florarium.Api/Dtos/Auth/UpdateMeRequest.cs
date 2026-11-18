using System.ComponentModel.DataAnnotations;

namespace Florarium.Api.Dtos.Auth;

/// <summary>Необязательные поля для обновления профиля.</summary>
public class UpdateMeRequest
{
    /// <summary>Новое отображаемое имя.</summary>
    /// <example>Анна Смирнова</example>
    [MaxLength(200)]
    /// <summary>Свойство Name.</summary>
    /// <example>пример</example>
    public string? Name {get; set;}

    /// <summary>Новый номер телефона.</summary>
    /// <example>+79001234567</example>
    [MaxLength(20)]
    /// <summary>Свойство Phone.</summary>
    /// <example>пример</example>
    public string? Phone {get; set;}
}
