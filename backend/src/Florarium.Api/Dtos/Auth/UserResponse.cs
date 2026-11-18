namespace Florarium.Api.Dtos.Auth;

/// <summary>Публичные данные пользователя.</summary>
public class UserResponse
{
    /// <summary>Идентификатор пользователя.</summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    /// <summary>Свойство Id.</summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid Id { get; set; }
    /// <summary>Отображаемое имя.</summary>
    /// <example>Анна Иванова</example>
    /// <summary>Свойство Name.</summary>
    /// <example>пример</example>
    public string Name { get; set; } = null!;
    /// <summary>Адрес электронной почты.</summary>
    /// <example>anna@example.com</example>
    /// <summary>Свойство Email.</summary>
    /// <example>пример</example>
    public string Email { get; set; } = null!;
    /// <summary>Номер телефона.</summary>
    /// <example>+79001234567</example>
    /// <summary>Свойство Phone.</summary>
    /// <example>пример</example>
    public string? Phone { get; set; }
    /// <summary>Название роли пользователя.</summary>
    /// <example>Customer</example>
    /// <summary>Свойство Role.</summary>
    /// <example>пример</example>
    public string Role { get; set; } = null!;
    /// <summary>Дата регистрации в UTC.</summary>
    /// <example>2026-11-18T12:00:00Z</example>
    /// <summary>Свойство RegisteredAt.</summary>
    /// <example>2026-11-18T12:00:00Z</example>
    public DateTime RegisteredAt { get; set; }
}
