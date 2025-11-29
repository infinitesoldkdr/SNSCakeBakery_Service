using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SNSCakeBakery_Service.Data;
using SNSCakeBakery_Service.Models;
using SNSCakeBakery_Service.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SNSCakeBakery_Service.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        public UserService(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        // ---------------------------
        // Register
        // ---------------------------
        public async Task<User?> RegisterAsync( string password, string email)
        {
            if (await _db.Users.AnyAsync(u => u.Email == email))
                return null;

            var hashed = BCrypt.Net.BCrypt.HashPassword(password);

            var newUser = new User
            {
                //Username = username,
                PasswordHash = hashed,
                Email = email
            };

            _db.Users.Add(newUser);
            await _db.SaveChangesAsync();

            return newUser;
        }

        // ---------------------------
        // Login (returns JWT token)
        // ---------------------------
        public async Task<string?> LoginAsync(string email, string password)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return null;

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;

            return GenerateJwt(user);
        }

        // ---------------------------
        // Get User By ID
        // ---------------------------
        public async Task<User?> GetUserByIdAsync(string id)
        {
            return await _db.Users.FindAsync(id);
        }

        // ---------------------------
        // Get User By Email
        // ---------------------------
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        // ---------------------------
        // JWT Generator
        // ---------------------------
        private string GenerateJwt(User user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"])
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
