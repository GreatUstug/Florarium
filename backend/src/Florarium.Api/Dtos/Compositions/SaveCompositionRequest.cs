using System.ComponentModel.DataAnnotations;

namespace Florarium.Api.Dtos.Compositions;

public class SaveCompositionRequest
{
    [Required]
    [MinLength(1, ErrorMessage = "Букет не может быть пустым")]
    [MaxLength(30, ErrorMessage = "Не больше 30 позиций")]
    public List<QuoteItemRequest> Items { get; set; } = [];

    public Guid? PackagingId { get; set; }
    public Guid? RibbonId { get; set; }

    [MaxLength(500)]
    public string? CardText { get; set; }
}