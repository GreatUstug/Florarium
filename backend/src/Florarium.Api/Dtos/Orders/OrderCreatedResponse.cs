namespace Florarium.Api.Dtos.Orders;

/// <summary>Модель данных OrderCreatedResponse.</summary>
public class OrderCreatedResponse
{
    /// <summary>Свойство Id.</summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid Id { get; set; }
    /// <summary>Свойство Number.</summary>
    /// <example>пример</example>
    public string Number { get; set; } = null!;
    /// <summary>Свойство Status.</summary>
    /// <example>пример</example>
    public string Status { get; set; } = null!;
    /// <summary>Свойство Total.</summary>
    /// <example>100.00</example>
    public decimal Total { get; set; }
    /// <summary>Свойство PaymentMethod.</summary>
    /// <example>пример</example>
    public string PaymentMethod { get; set; } = null!;
    /// <summary>Свойство PaymentRequired.</summary>
    /// <example>true</example>
    public bool PaymentRequired { get; set; }
}
