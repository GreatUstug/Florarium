namespace Florarium.Domain.Entities;

public class Flower
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
    public string? Variety { get; set; }
    public string? LatinName { get; set; }

    public decimal PricePerStem { get; set; }
    public string? ColorFamily { get; set; }
    public string? PhotoUrl { get; set; }

    public bool IsActive { get; set; } = true;

    public Stock Stock { get; set; } = null!;
    public ICollection<CompositionItem> CompositionItems { get; set; } = [];
    public ICollection<SupplyBatch> SupplyBatches { get; set; } = [];
    public ICollection<StockReservation> StockReservations { get; set; } = [];
}
