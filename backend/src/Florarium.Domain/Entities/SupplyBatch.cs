namespace Florarium.Domain.Entities;

public class SupplyBatch
{
    public Guid Id { get; set; }
    public Guid FlowerId { get; set; }
    public DateTime ReceivedAt { get; set; }
    public int Qty { get; set; }
    public DateOnly? FreshUntil { get; set; }
    public decimal PurchasePrice { get; set; }

    public Flower Flower { get; set; } = null!;
}