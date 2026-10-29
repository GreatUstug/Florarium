namespace Florarium.Api.Dtos.Compositions;

public class QuoteRequest
{
    public List<QuoteItemRequest> Items { get; set; } = new();
    public Guid? PackagingId { get; set; }
    public Guid? RibbonId { get; set; }
}

public class QuoteItemRequest
{
    public Guid FlowerId { get; set; }
    public int Qty { get; set; }
}