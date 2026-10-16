using Florarium.Domain.Enums;

namespace Florarium.Domain.Entities;

public class Composition
{
    public Guid Id { get; set; }
    public Guid AuthorId { get; set; }
    public Guid? PackagingId { get; set; }
    public Guid? RibbonId { get; set; }
    public decimal Price { get; set; }
    public string? CardText { get; set; }
    public string? PhotoUrl { get; set; }
    public CompositionSource Source { get; set; } = CompositionSource.Custom;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User Author { get; set; } = null!;
    public Packaging? Packaging { get; set; }
    public Ribbon? Ribbon { get; set; }
    public ICollection<CompositionItem> Items { get; set; } = new List<CompositionItem>();
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}