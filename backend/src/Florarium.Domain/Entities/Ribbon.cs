namespace Florarium.Domain.Entities;

/// <summary>Вариант ленты для оформления букета.</summary>
public class Ribbon
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string? ColorHex { get; set; }

    public ICollection<Composition> Compositions { get; set; } = [];
}