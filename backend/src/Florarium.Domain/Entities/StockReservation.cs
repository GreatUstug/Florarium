namespace Florarium.Domain.Entities;

public class StockReservation
{
    public Guid Id { get; set; }
    public Guid FlowerId { get; set; }
    public Guid CartItemId { get; set; }
    public int Qty { get; set; }
    public DateTime ExpiresAt { get; set; }

    public Flower Flower { get; set; } = null!;
    public CartItem CartItem { get; set; } = null!;
}