namespace Florarium.Domain.Entities;

/// <summary>Корзина одного пользователя.</summary>
public class Cart
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public User Customer { get; set; } = null!;
    public ICollection<CartItem> Items { get; set; } = [];
}