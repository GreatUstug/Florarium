using System.Security.Claims;

using Florarium.Api.Dtos.Auth;
using Florarium.Api.Services;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Florarium.Api.Controllers;

/// <summary>Регистрация, вход и выход из учетной записи.</summary>
[ApiController]
[Route("api/auth")]
public class AuthController(AuthService authService) : ControllerBase
{
    private readonly AuthService _authService = authService;

    /// <summary>Регистрирует нового пользователя.</summary>
    /// <remarks>При успешной регистрации создается cookie-сессия. Email должен быть уникальным.</remarks>
    /// <param name="request">Данные нового пользователя.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Созданный профиль пользователя.</returns>
    /// <response code="201">Пользователь создан и выполнен вход.</response>
    /// <response code="409">Пользователь с указанным email уже существует.</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserResponse>> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var user = await _authService.RegisterAsync(request, ct);
        await SignInUserAsync(user.Id, user.Name, user.Role);
        return StatusCode(201, user);
    }

    /// <summary>Выполняет вход по email и паролю.</summary>
    /// <remarks>Успешный вход устанавливает защищенную cookie-сессию.</remarks>
    /// <param name="request">Учетные данные пользователя.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Профиль авторизованного пользователя.</returns>
    /// <response code="200">Вход выполнен.</response>
    /// <response code="401">Email или пароль неверны.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserResponse>> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var user = await _authService.VerifyCredentialsAsync(request.Email, request.Password, ct);

        if (user == null)
        {
            var problem = new ProblemDetails { Title = "Неверный email или пароль", Status = 401 };
            problem.Extensions.Add("code", "invalid_credentials");
            return Unauthorized(problem);
        }

        await SignInUserAsync(user.Id, user.Name, user.Role.Name);
        return Ok(_authService.MapToResponse(user));
    }

    /// <summary>Завершает cookie-сессию пользователя.</summary>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Результат выхода.</returns>
    /// <response code="204">Сессия завершена.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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