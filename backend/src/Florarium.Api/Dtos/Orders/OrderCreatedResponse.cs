namespace Florarium.Api.Dtos.Orders;

public class OrderCreatedResponse
{
    public Guid Id { get; set; }
    public string Number { get; set; } = null!;
    public string Status { get; set; } = null!;
    public decimal Total { get; set; }
    public string PaymentMethod { get; set; } = null!;
    public bool PaymentRequired { get; set; }
}