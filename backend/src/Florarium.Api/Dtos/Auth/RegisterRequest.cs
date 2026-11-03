using System.ComponentModel.DataAnnotations;

namespace Florarium.Api.Dtos.Auth;

public class RegisterRequest
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = null!;

    [Required, EmailAddress, MaxLength(200)]
    public string Email { get; set; } = null!;

    [Required, MinLength(8)]
    public string Password { get; set; } = null!;

    [MaxLength(20)]
    public string? Phone { get; set; }
}