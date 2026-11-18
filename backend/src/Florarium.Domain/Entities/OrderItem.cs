namespace Florarium.Domain.Entities;

/// <summary>Позиция заказа с купленным составом, количеством и ценой.</summary>
public class OrderItem
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid CompositionId { get; set; }
    public int Qty { get; set; }
    public decimal UnitPrice { get; set; }

    public Order Order { get; set; } = null!;
    public Composition Composition { get; set; } = null!;
    public Rating? Rating { get; set; }
}