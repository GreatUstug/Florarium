namespace Florarium.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public Guid RoleId { get; set; }
    public string Name { get; set; } = null!;
    public string? Phone { get; set; }
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

    public Role Role { get; set; } = null!;
    public Cart? Cart { get; set; }
    public ICollection<Address> Addresses { get; set; } = [];
    public ICollection<Order> Orders { get; set; } = [];
    public ICollection<Composition> Compositions { get; set; } = [];
    public ICollection<Rating> Ratings { get; set; } = [];
}