namespace Florarium.Domain.Entities;

/// <summary>Вариант упаковки букета со стоимостью.</summary>
public class Packaging
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public decimal Price { get; set; }

    public ICollection<Composition> Compositions { get; set; } = [];
}