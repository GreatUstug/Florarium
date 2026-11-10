using System.Security.Claims;
using Florarium.Api.Dtos.Auth;
using Florarium.Api.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Florarium.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(AuthService authService) : ControllerBase
{
    private readonly AuthService _authService = authService;

    [HttpPost("register")]
    public async Task<ActionResult<UserResponse>> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        try
        {
            var user = await _authService.RegisterAsync(request, ct);
            await SignInUserAsync(user.Id, user.Name, user.Role);
            return StatusCode(201, user);
        }
        catch (InvalidOperationException ex)
        {
            var problem = new ProblemDetails {Title = ex.Message, Status = 409};
            problem.Extensions.Add("code", "email_taken");
            return Conflict(problem);
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserResponse>> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var user = await _authService.VerifyCredentialsAsync(request.Email, request.Password, ct);

        if (user == null)
        {
            var problem = new ProblemDetails {Title = "Неверный email или пароль", Status = 401};
            problem.Extensions.Add("code", "invalid_credentials");
            return Unauthorized(problem);
        }

        await SignInUserAsync(user.Id, user.Name, user.Role.Name);
        return Ok(_authService.MapToResponse(user));
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return NoContent();
    }

    private async Task SignInUserAsync(Guid userId, string name, string role)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Name, name),
            new(ClaimTypes.Role, role)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    }
}