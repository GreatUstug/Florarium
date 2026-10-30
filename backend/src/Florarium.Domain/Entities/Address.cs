namespace Florarium.Domain.Entities;

public class Address
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string? Label { get; set; }
    public string Street { get; set; } = null!;
    public string? Apartment { get; set; }
    public string? Entrance { get; set; }
    public string? Floor { get; set; }
    public string? Intercom { get; set; }

    public User Customer { get; set; } = null!;
}