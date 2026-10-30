using System.ComponentModel.DataAnnotations;

namespace Florarium.Api.Dtos.Compositions;

public class QuoteRequest
{
    [Required]
    [MinLength(1, ErrorMessage = "Букет не может быть пустым")]
    [MaxLength(30, ErrorMessage = "Не больше 30 позиций")]
    public List<QuoteItemRequest> Items { get; set; } = [];
    
    public Guid? PackagingId { get; set; }
    public Guid? RibbonId { get; set; }
}

public class QuoteItemRequest
{
    [Required]
    public Guid FlowerId { get; set; }
    
    [Range(1, 200, ErrorMessage = "Количество должно быть от 1 до 200")]
    public int Qty { get; set; }
}