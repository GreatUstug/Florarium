namespace Florarium.Domain.Entities;

public class Rating
{
    public Guid Id { get; set; }
    public Guid OrderItemId { get; set; }
    public Guid CustomerId { get; set; }
    public short Stars { get; set; }
    public string[]? Tags { get; set; }
    public string? Text { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public OrderItem OrderItem { get; set; } = null!;
    public User Customer { get; set; } = null!;
}