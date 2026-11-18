using System.ComponentModel.DataAnnotations;
using Florarium.Domain.Enums;

namespace Florarium.Api.Dtos.Orders;

/// <summary>Данные для создания заказа.</summary>
public class CreateOrderRequest
{
    /// <summary>Идентификатор сохраненного адреса доставки.</summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    [Required]
    /// <summary>Свойство AddressId.</summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid AddressId { get; set; }

    /// <summary>Имя получателя; если не указано, используется имя пользователя.</summary>
    /// <example>Анна Иванова</example>
    [MaxLength(200)]
    /// <summary>Свойство RecipientName.</summary>
    /// <example>пример</example>
    public string? RecipientName { get; set; }

    [MaxLength(20)]
    /// <summary>Свойство RecipientPhone.</summary>
    /// <example>пример</example>
    public string? RecipientPhone { get; set; }

    /// <summary>Желаемая дата доставки.</summary>
    /// <example>2026-11-20</example>
    [Required]
    /// <summary>Свойство Date.</summary>
    /// <example>2026-11-20</example>
    public DateOnly Date { get; set; }

    [Required, MaxLength(50)]
    /// <summary>Свойство Slot.</summary>
    /// <example>пример</example>
    public string Slot { get; set; } = null!;

    /// <summary>Комментарий курьеру.</summary>
    /// <example>Позвонить за 10 минут до приезда</example>
    [MaxLength(500)]
    /// <summary>Свойство CourierNote.</summary>
    /// <example>пример</example>
    public string? CourierNote { get; set; }

    [Required]
    /// <summary>Свойство PaymentMethod.</summary>
    /// <example>пример</example>
    public PaymentMethod PaymentMethod { get; set; }
}
