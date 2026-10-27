namespace Florarium.Domain.Entities;

public class CartItem
{
    public Guid Id { get; set; }
    public Guid CartId { get; set; }
    public Guid CompositionId { get; set; }
    public int Qty { get; set; }
    public decimal UnitPrice { get; set; }

    public Cart Cart { get; set; } = null!;
    public Composition Composition { get; set; } = null!;
    public ICollection<StockReservation> Reservations { get; set; } = [];
}