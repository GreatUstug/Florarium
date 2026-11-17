namespace Florarium.Api.Dtos.Compositions;

public class CompositionListDto
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? PhotoUrl { get; set; }
    public decimal Price { get; set; }
    public List<string> FlowersPreview { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public CompositionAvailabilityDto Availability { get; set; } = new();
    public int OrdersCount { get; set; }
    public bool IsEditable { get; set; }
}

public class CompositionAvailabilityDto
{
    public bool InStock { get; set; }
    public int MaxQty { get; set; }
}