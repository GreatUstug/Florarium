namespace Florarium.Api.Dtos.Cart;

public class CartResponse
{
    public List<CartItemDto> Items { get; set; } = [];
    public int Count { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Total { get; set; }
    public DateTime? ReservedUntil { get; set; }
}

public class CartItemDto
{
    public Guid Id { get; set; }
    public CartCompositionDto Composition { get; set; } = null!;
    public int Qty { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
    public string Status { get; set; } = "reserved";
    public int MaxQty { get; set; } = 50;
}

public class CartCompositionDto
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? PhotoUrl { get; set; }
    public string Source { get; set; } = null!;
}