namespace Florarium.Api.Dtos.Orders;

public class OrderListDto
{
    public Guid Id { get; set; }
    public string Number { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public decimal Total { get; set; }
    public int ItemsCount { get; set; }
    public string? PreviewPhotoUrl { get; set; }
    public OrderDeliveryDto? Delivery { get; set; }
}

public class OrderDeliveryDto
{
    public DateOnly Date { get; set; }
    public string? Slot { get; set; }
    public string Status { get; set; } = null!;
}