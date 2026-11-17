using System.ComponentModel.DataAnnotations;
using Florarium.Domain.Enums;

namespace Florarium.Api.Dtos.Orders;

public class CreateOrderRequest
{
    [Required]
    public Guid AddressId { get; set; }

    [MaxLength(200)]
    public string? RecipientName { get; set; }

    [MaxLength(20)]
    public string? RecipientPhone { get; set; }

    [Required]
    public DateOnly Date { get; set; }

    [Required, MaxLength(50)]
    public string Slot { get; set; } = null!;

    [MaxLength(500)]
    public string? CourierNote { get; set; }

    [Required]
    public PaymentMethod PaymentMethod { get; set; }
}