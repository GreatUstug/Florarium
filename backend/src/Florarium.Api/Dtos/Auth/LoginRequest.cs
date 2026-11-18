using System.ComponentModel.DataAnnotations;

namespace Florarium.Api.Dtos.Auth;

/// <summary>Учетные данные для входа.</summary>
public class LoginRequest
{
    /// <summary>Email пользователя.</summary>
    /// <example>anna@example.com</example>
    [Required, EmailAddress]
    /// <summary>Свойство Email.</summary>
    /// <example>пример</example>
    public string Email { get; set; } = null!;

    /// <summary>Пароль пользователя.</summary>
    /// <example>securePassword123</example>
    [Required]
    /// <summary>Свойство Password.</summary>
    /// <example>пример</example>
    public string Password { get; set; } = null!;
}
