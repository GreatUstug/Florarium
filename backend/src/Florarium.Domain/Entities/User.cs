namespace Florarium.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Phone { get; set; }
    public string Email { get; set; } = null!;
    public DateTime RegisteredAt  { get; set; } =  DateTime.UtcNow;

    public Role Role { get; set; } = null!;
    public ICollection<Address> Addresses { get; set; } = new List<Address>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Composition> Compositions { get; set; } = new List<Composition>();
    public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
}