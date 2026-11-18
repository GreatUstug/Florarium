namespace Florarium.Api.Dtos.Orders;

/// <summary>Модель данных OrderListDto.</summary>
public class OrderListDto
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
    /// <summary>Свойство CreatedAt.</summary>
    /// <example>2026-11-18T12:00:00Z</example>
    public DateTime CreatedAt { get; set; }
    /// <summary>Свойство Total.</summary>
    /// <example>100.00</example>
    public decimal Total { get; set; }
    /// <summary>Свойство ItemsCount.</summary>
    /// <example>1</example>
    public int ItemsCount { get; set; }
    /// <summary>Свойство PreviewPhotoUrl.</summary>
    /// <example>пример</example>
    public string? PreviewPhotoUrl { get; set; }
    /// <summary>Свойство Delivery.</summary>
    /// <example>пример</example>
    public OrderDeliveryDto? Delivery { get; set; }
}

/// <summary>Модель данных OrderDeliveryDto.</summary>
public class OrderDeliveryDto
{
    /// <summary>Свойство Date.</summary>
    /// <example>2026-11-20</example>
    public DateOnly Date { get; set; }
    /// <summary>Свойство Slot.</summary>
    /// <example>пример</example>
    public string? Slot { get; set; }
    /// <summary>Свойство Status.</summary>
    /// <example>пример</example>
    public string Status { get; set; } = null!;
}
