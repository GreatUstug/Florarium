namespace Florarium.Domain.Entities;

/// <summary>Актуальное количество и доступность конкретного цветка на складе.</summary>
public class Stock
{
    public Guid FlowerId { get; set; }
    public int Available { get; set; }
    public int Reserved { get; set; }
    public DateOnly? BackInStockAt { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public uint Version { get; set; }

    public Flower Flower { get; set; } = null!;
}