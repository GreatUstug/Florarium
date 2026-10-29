namespace Florarium.Api.Dtos.Dictionaries;

public class PackagingDto
{
    public Guid Id {get; set;}
    public string Title {get; set;} = null!;
    public decimal Price {get; set;}
}