using System.ComponentModel.DataAnnotations;

namespace Florarium.Api.Dtos.Auth;

/// <summary>Данные для регистрации нового пользователя.</summary>
public class RegisterRequest
{
    /// <summary>Отображаемое имя пользователя.</summary>
    /// <example>Анна Иванова</example>
    [Required, MaxLength(200)]
    /// <summary>Свойство Name.</summary>
    /// <example>пример</example>
    public string Name { get; set; } = null!;

    /// <summary>Уникальный адрес электронной почты.</summary>
    /// <example>anna@example.com</example>
    [Required, EmailAddress, MaxLength(200)]
    /// <summary>Свойство Email.</summary>
    /// <example>пример</example>
    public string Email { get; set; } = null!;

    /// <summary>Пароль пользователя длиной не менее восьми символов.</summary>
    /// <example>securePassword123</example>
    [Required, MinLength(8)]
    /// <summary>Свойство Password.</summary>
    /// <example>пример</example>
    public string Password { get; set; } = null!;

    /// <summary>Необязательный номер телефона.</summary>
    /// <example>+79001234567</example>
    [MaxLength(20)]
    /// <summary>Свойство Phone.</summary>
    /// <example>пример</example>
    public string? Phone { get; set; }
}
