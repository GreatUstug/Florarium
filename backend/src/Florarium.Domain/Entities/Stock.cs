namespace Florarium.Domain.Entities;

public class Stock
{
    public Guid FlowerId { get; set; }
    public int Available { get; set; }
    public int Reserved { get; set; }
    public DateOnly? BackInStockAt { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Flower Flower { get; set; } = null!;
}