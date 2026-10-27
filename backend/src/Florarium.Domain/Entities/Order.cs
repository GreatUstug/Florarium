using Florarium.Domain.Enums;

namespace Florarium.Domain.Entities;

public class Order
{
    public Guid Id { get; set; }
    public string Number { get; set; } = null!;
    public Guid CustomerId { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.New;
    public decimal Subtotal { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User Customer { get; set; } = null!;
    public ICollection<OrderItem> Items { get; set; } = [];
    public ICollection<Payment> Payments { get; set; } = [];
    public Delivery? Delivery { get; set; }
}