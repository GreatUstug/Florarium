namespace Florarium.Api.Dtos.Orders;

public class OrderDetailDto
{
    public Guid Id { get; set; }
    public string Number { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
    
    public List<OrderItemDto> Items { get; set; } = [];
    public OrderDeliveryFullDto? Delivery { get; set; }
    public List<OrderPaymentDto> Payments { get; set; } = [];

    public bool CanCancel { get; set; }
    public bool CanRepeat { get; set; }
}

public class OrderItemDto
{
    public Guid Id { get; set; }
    public OrderCompositionDto Composition { get; set; } = null!;
    public int Qty { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
    public bool CanRate { get; set; }
}

public class OrderCompositionDto
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? PhotoUrl { get; set; }
    public string Source { get; set; } = null!;
}

public class OrderDeliveryFullDto
{
    public string? RecipientName { get; set; }
    public DateOnly Date { get; set; }
    public string? Slot { get; set; }
    public string? CourierNote { get; set; }
    public string Status { get; set; } = null!;
    public DateTime? DeliveredAt { get; set; }
}

public class OrderPaymentDto
{
    public Guid Id { get; set; }
    public string Method { get; set; } = null!;
    public string Status { get; set; } = null!;
    public decimal Amount { get; set; }
    public DateTime? PaidAt { get; set; }
}