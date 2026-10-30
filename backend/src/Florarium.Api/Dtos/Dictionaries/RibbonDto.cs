namespace Florarium.Api.Dtos.Dictionaries;

public class RibbonDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string? ColorHex { get; set; }
}