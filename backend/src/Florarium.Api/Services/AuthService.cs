using Florarium.Api.Dtos.Auth;
using Florarium.Data;
using Florarium.Domain.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Florarium.Api.Services;

public class AuthService(AppDbContext db)
{
    private readonly AppDbContext _db = db;
    private readonly PasswordHasher<User> _passwordHasher = new();

    public async Task<UserResponse> RegisterAsync(RegisterRequest request, CancellationToken ct)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        if (await _db.Users.AnyAsync(u => u.Email == email, ct))
            throw new InvalidOperationException("Пользователь с таким email уже существует");

        var clientRole = await _db.Roles.FirstOrDefaultAsync(r => r.Name == "client", ct) 
            ?? throw new Exception("Роль 'client' не найдена в БД");

        var user = new User
        {
            Name = request.Name.Trim(),
            Email = email,
            Phone = request.Phone,
            Role = clientRole
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);

        return MapToResponse(user);
    }

    public async Task<User> VerifyCredentialsAsync(string email, string password, CancellationToken ct)
    {
        email = email.Trim().ToLowerInvariant();

        var user = await _db.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == email, ct);

        if (user == null) return null!;

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

        if (result == PasswordVerificationResult.Failed)
            return null!;

        return user;
    }

    public UserResponse MapToResponse(User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Phone = user.Phone,
            Role = user.Role?.Name ?? "client",
            RegisteredAt = user.RegisteredAt
        };
    }
}