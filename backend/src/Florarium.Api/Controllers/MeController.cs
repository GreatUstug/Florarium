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

[ApiController]
[Route("api/me")]
[Authorize]
public class MeController(AppDbContext db, AddressesService addressesService, CompositionsService compositionsService) : ControllerBase
{
    private readonly AppDbContext _db = db;
    private readonly AddressesService _addressesService = addressesService;
    private readonly CompositionsService _compositionsService = compositionsService;

    [HttpGet]
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

    [HttpPatch]
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

    [HttpGet("addresses")]
    public async Task<ActionResult<List<AddressDto>>> GetAddresses(CancellationToken ct)
    {
        var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();
        return Ok(await _addressesService.GetUserAddressesAsync(userId, ct));
    }

    [HttpPost("addresses")]
    public async Task<ActionResult<AddressDto>> CreateAdress([FromBody] CreateAddressRequest request, CancellationToken ct)
    {
        var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();
        var result = await _addressesService.CreateAddressAsync(userId, request, ct);
        return StatusCode(201, result);
    }

    [HttpGet("compositions")]
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