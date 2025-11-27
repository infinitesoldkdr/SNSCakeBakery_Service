using Microsoft.EntityFrameworkCore;
using SNSCakeBakery_Service.Data;
using SNSCakeBakery_Service.DTOs.Auth;
using SNSCakeBakery_Service.Helpers;
using SNSCakeBakery_Service.Models;
using SNSCakeBakery_Service.Services.Interfaces;

namespace SNSCakeBakery_Service.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly JwtTokenGenerator _jwt;

    public AuthService(AppDbContext db, JwtTokenGenerator jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    public async Task<AuthResponse?> Register(RegisterRequest request)
    {
        if (await _db.Users.AnyAsync(x => x.Email == request.Email))
            return null;

        var user = new User
        {
            Email = request.Email,
            FullName = request.FullName,
            PasswordHash = PasswordHasher.Hash(request.Password)
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return new AuthResponse
        {
            Email = user.Email,
            FullName = user.FullName,
            Token = _jwt.GenerateToken(user)
        };
    }

    public async Task<AuthResponse?> Login(LoginRequest request)
    {
        var user = await _db.Users.SingleOrDefaultAsync(x => x.Email == request.Email);
        if (user == null) return null;

        if (!PasswordHasher.Verify(request.Password, user.PasswordHash))
            return null;

        return new AuthResponse
        {
            Email = user.Email,
            FullName = user.FullName,
            Token = _jwt.GenerateToken(user)
        };
    }
}
