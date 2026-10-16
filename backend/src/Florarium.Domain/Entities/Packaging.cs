namespace Florarium.Domain.Entities;

public class Packaging
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public decimal Price { get; set; }

    public ICollection<Composition> Compositions { get; set; } = new List<Composition>();
}