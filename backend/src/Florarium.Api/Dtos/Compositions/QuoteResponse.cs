namespace Florarium.Api.Dtos.Compositions;

public class QuoteResponse
{
    public List<QuoteItemResponse> Items { get; set; } = [];
    public decimal FlowersTotal { get; set; }
    public decimal PackagingPrice { get; set; }
    public decimal Total { get; set; }
    public List<QuoteWarningDto> Warnings { get; set; } = [];
}

public class QuoteItemResponse
{
    public Guid FlowerId { get; set; }
    public string FlowerName { get; set; } = null!;
    public int Qty { get; set; }
    public decimal PricePerStem { get; set; }
    public decimal LineTotal { get; set; }
}

public class QuoteWarningDto
{
    public string Code { get; set; } = "out_of_stock";
    public Guid FlowerId { get; set; }
    public int Requested { get; set; }
    public int Available { get; set; }
}