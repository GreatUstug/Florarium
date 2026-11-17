using System.ComponentModel.DataAnnotations;

namespace Florarium.Api.Dtos.Cart;

public class AddToCartRequest
{
    [Required]
    public Guid CompositionId { get; set; }
    
    [Range(1, 50)]
    public int Qty { get; set; } = 1;
}

public class UpdateCartItemRequest
{
    [Range(1, 50)]
    public int Qty { get; set; }
}