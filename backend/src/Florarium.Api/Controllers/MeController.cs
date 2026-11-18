using Florarium.Api.Dtos.Auth;
using Florarium.Api.Dtos.Common;
using Florarium.Api.Dtos.Compositions;
using Florarium.Api.Dtos.Me;
using Florarium.Api.Services;
using Florarium.Data;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Florarium.Api.Controllers;

/// <summary>Профиль пользователя, адреса доставки и индивидуальные составы.</summary>
/// <remarks>Все методы требуют cookie-аутентификации.</remarks>
[ApiController]
[Route("api/me")]
[Authorize]
public class MeController(AppDbContext db, AddressesService addressesService, CompositionsService compositionsService) : ControllerBase
{
    private readonly AppDbContext _db = db;
    private readonly AddressesService _addressesService = addressesService;
    private readonly CompositionsService _compositionsService = compositionsService;

    /// <summary>Возвращает профиль текущего пользователя.</summary>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Данные профиля.</returns>
    /// <response code="200">Профиль получен.</response>
    /// <response code="401">Сессия недействительна или пользователь не найден.</response>
    [HttpGet]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserResponse>> GetMe(CancellationToken ct)
    {
        var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        var user = await _db.Users
            .Include(u => u.Role)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        return user == null
            ? Unauthorized()
            : Ok(new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role.Name,
                RegisteredAt = user.RegisteredAt
            });
    }

    /// <summary>Обновляет имя и телефон профиля.</summary>
    /// <param name="request">Новые необязательные поля профиля.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Обновленный профиль.</returns>
    /// <response code="200">Профиль обновлен.</response>
    /// <response code="401">Сессия недействительна или пользователь не найден.</response>
    [HttpPatch]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserResponse>> UpdateMe([FromBody] UpdateMeRequest request, CancellationToken ct)
    {
        var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out var userId)) return Unauthorized();

        var user = await _db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == userId, ct);
        if (user == null) return Unauthorized();

        if (!string.IsNullOrWhiteSpace(request.Name)) user.Name = request.Name.Trim();
        if (request.Phone != null) user.Phone = request.Phone.Trim();

        await _db.SaveChangesAsync(ct);

        return Ok(new UserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Phone = user.Phone,
            Role = user.Role.Name,
            RegisteredAt = user.RegisteredAt
        });
    }

    /// <summary>Возвращает сохраненные адреса доставки пользователя.</summary>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Список адресов.</returns>
    /// <response code="200">Адреса получены.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    [HttpGet("addresses")]
    [ProducesResponseType(typeof(List<AddressDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<AddressDto>>> GetAddresses(CancellationToken ct)
    {
        var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();
        return Ok(await _addressesService.GetUserAddressesAsync(userId, ct));
    }

    /// <summary>Создает новый адрес доставки пользователя.</summary>
    /// <param name="request">Данные адреса.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Созданный адрес.</returns>
    /// <response code="201">Адрес создан.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    [HttpPost("addresses")]
    [ProducesResponseType(typeof(AddressDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AddressDto>> CreateAdress([FromBody] CreateAddressRequest request, CancellationToken ct)
    {
        var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();
        var result = await _addressesService.CreateAddressAsync(userId, request, ct);
        return StatusCode(201, result);
    }

    /// <summary>Возвращает индивидуальные композиции текущего пользователя.</summary>
    /// <param name="page">Номер страницы.</param>
    /// <param name="pageSize">Размер страницы.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Постраничный список составов.</returns>
    /// <response code="200">Составы получены.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    [HttpGet("compositions")]
    [ProducesResponseType(typeof(PagedResponse<CompositionListDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResponse<CompositionListDto>>> GetMyCompositions(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 12,
    CancellationToken ct = default)
    {
        var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();
        return Ok(await _compositionsService.GetUserCompositionsAsync(userId, page, pageSize, ct));
    }
}