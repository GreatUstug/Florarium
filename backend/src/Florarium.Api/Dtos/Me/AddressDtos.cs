using System.ComponentModel.DataAnnotations;

namespace Florarium.Api.Dtos.Me;

public class AddressDto
{
    public Guid Id { get; set; }
    public string? Label { get; set; }
    public string Street { get; set; } = null!;
    public string? Apartment { get; set; }
    public string? Entrance { get; set; }
    public string? Floor { get; set; }
    public string? Intercom { get; set; }
}

public class CreateAddressRequest
{
    [MaxLength(50)]
    public string? Label { get; set; }
    
    [Required, MaxLength(300)]
    public string Street { get; set; } = null!;
    
    [MaxLength(20)]
    public string? Apartment { get; set; }
    
    [MaxLength(20)]
    public string? Entrance { get; set; }
    
    [MaxLength(20)]
    public string? Floor { get; set; }
    
    [MaxLength(20)]
    public string? Intercom { get; set; }
}