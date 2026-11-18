namespace Florarium.Api.Dtos.Orders;

/// <summary>Модель данных OrderDetailDto.</summary>
public class OrderDetailDto
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
    /// <summary>Свойство Subtotal.</summary>
    /// <example>100.00</example>
    public decimal Subtotal { get; set; }
    /// <summary>Свойство Discount.</summary>
    /// <example>100.00</example>
    public decimal Discount { get; set; }
    /// <summary>Свойство Total.</summary>
    /// <example>100.00</example>
    public decimal Total { get; set; }
    
    /// <summary>Свойство Items.</summary>
    /// <example>[]</example>
    public List<OrderItemDto> Items { get; set; } = [];
    /// <summary>Свойство Delivery.</summary>
    /// <example>пример</example>
    public OrderDeliveryFullDto? Delivery { get; set; }
    /// <summary>Свойство Payments.</summary>
    /// <example>[]</example>
    public List<OrderPaymentDto> Payments { get; set; } = [];

    /// <summary>Свойство CanCancel.</summary>
    /// <example>true</example>
    public bool CanCancel { get; set; }
    /// <summary>Свойство CanRepeat.</summary>
    /// <example>true</example>
    public bool CanRepeat { get; set; }
}

/// <summary>Модель данных OrderItemDto.</summary>
public class OrderItemDto
{
    /// <summary>Свойство Id.</summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid Id { get; set; }
    /// <summary>Свойство Composition.</summary>
    /// <example>пример</example>
    public OrderCompositionDto Composition { get; set; } = null!;
    /// <summary>Свойство Qty.</summary>
    /// <example>1</example>
    public int Qty { get; set; }
    /// <summary>Свойство UnitPrice.</summary>
    /// <example>100.00</example>
    public decimal UnitPrice { get; set; }
    /// <summary>Свойство LineTotal.</summary>
    /// <example>100.00</example>
    public decimal LineTotal { get; set; }
    /// <summary>Свойство CanRate.</summary>
    /// <example>true</example>
    public bool CanRate { get; set; }
}

/// <summary>Модель данных OrderCompositionDto.</summary>
public class OrderCompositionDto
{
    /// <summary>Свойство Id.</summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid Id { get; set; }
    /// <summary>Свойство Title.</summary>
    /// <example>пример</example>
    public string? Title { get; set; }
    /// <summary>Свойство PhotoUrl.</summary>
    /// <example>пример</example>
    public string? PhotoUrl { get; set; }
    /// <summary>Свойство Source.</summary>
    /// <example>пример</example>
    public string Source { get; set; } = null!;
}

/// <summary>Модель данных OrderDeliveryFullDto.</summary>
public class OrderDeliveryFullDto
{
    /// <summary>Свойство RecipientName.</summary>
    /// <example>пример</example>
    public string? RecipientName { get; set; }
    /// <summary>Свойство Date.</summary>
    /// <example>2026-11-20</example>
    public DateOnly Date { get; set; }
    /// <summary>Свойство Slot.</summary>
    /// <example>пример</example>
    public string? Slot { get; set; }
    /// <summary>Свойство CourierNote.</summary>
    /// <example>пример</example>
    public string? CourierNote { get; set; }
    /// <summary>Свойство Status.</summary>
    /// <example>пример</example>
    public string Status { get; set; } = null!;
    /// <summary>Свойство DeliveredAt.</summary>
    /// <example>2026-11-18T12:00:00Z</example>
    public DateTime? DeliveredAt { get; set; }
}

/// <summary>Модель данных OrderPaymentDto.</summary>
public class OrderPaymentDto
{
    /// <summary>Свойство Id.</summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid Id { get; set; }
    /// <summary>Свойство Method.</summary>
    /// <example>пример</example>
    public string Method { get; set; } = null!;
    /// <summary>Свойство Status.</summary>
    /// <example>пример</example>
    public string Status { get; set; } = null!;
    /// <summary>Свойство Amount.</summary>
    /// <example>100.00</example>
    public decimal Amount { get; set; }
    /// <summary>Свойство PaidAt.</summary>
    /// <example>2026-11-18T12:00:00Z</example>
    public DateTime? PaidAt { get; set; }
}
