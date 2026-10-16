using Florarium.Domain.Enums;

namespace Florarium.Domain.Entities;

public class Delivery
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string? RecipientName { get; set; }
    public DateOnly Date { get; set; }
    public string? Slot { get; set; }
    public string? CourierNote { get; set; }
    public DeliveryStatus Status { get; set; } = DeliveryStatus.Pending;
    public DateTime? DeliveredAt { get; set; }

    public Order Order { get; set; } = null!;
}