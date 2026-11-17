namespace Florarium.Api.Dtos.Cart;

public class RepeatOrderResponse
{
    public CartResponse Cart { get; set; } = null!;
    public List<SkippedItemDto> Skipped { get; set; } = new();
}

public class SkippedItemDto
{
    public Guid CompositionId { get; set; }
    public string? Title { get; set; }
    public string Reason { get; set; } = null!; // out_of_stock, flower_inactive
}