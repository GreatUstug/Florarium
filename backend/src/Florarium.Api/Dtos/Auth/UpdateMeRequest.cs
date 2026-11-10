using System.ComponentModel.DataAnnotations;

namespace Florarium.Api.Dtos.Auth;

public class UpdateMeRequest
{
    [MaxLength(200)]
    public string? Name {get; set;}

    [MaxLength(20)]
    public string? Phone {get; set;}
}