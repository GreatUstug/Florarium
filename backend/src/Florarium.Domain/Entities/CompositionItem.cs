namespace Florarium.Domain.Entities;

public class CompositionItem
{
    public Guid Id { get; set; }
    public Guid CompositionId { get; set; }
    public Guid FlowerId { get; set; }
    public int Qty { get; set; }

    public Composition Composition { get; set; } = null!;
    public Flower Flower { get; set; } = null!;
}