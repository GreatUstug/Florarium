using Florarium.Api.Dtos.Auth;
using Florarium.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Florarium.Api.Controllers;

[ApiController]
[Route("api/me")]
[Authorize]
public class MeController(AppDbContext db) : ControllerBase
{
    private readonly AppDbContext _db = db;

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
}