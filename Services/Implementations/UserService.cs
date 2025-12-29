using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SNSCakeBakery_Service.Data;
using SNSCakeBakery_Service.DTO.Login;
using SNSCakeBakery_Service.DTO.Register;
using SNSCakeBakery_Service.DTO.Service;
using SNSCakeBakery_Service.DTO.User;
using SNSCakeBakery_Service.DTOs.Auth;
using SNSCakeBakery_Service.Helpers;
using SNSCakeBakery_Service.Models;
using SNSCakeBakery_Service.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SNSCakeBakery_Service.Services.Helpers;
using Microsoft.JSInterop.Infrastructure;

namespace SNSCakeBakery_Service.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        private readonly JwtTokenGenerator _jwt;

        public UserService(AppDbContext db, IConfiguration config, JwtTokenGenerator jwt)
        {
            _db = db;
            _config = config;
            _jwt = jwt;
        }

        // -------------------------------------------------------
        // Register
        // -------------------------------------------------------
        public async Task<ServiceResponse> RegisterAsync(RegisterRequestDto request)
        {
            var exists = await _db.Users.AnyAsync(u => u.Email == request.Email);

            if (exists)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Email already registered."
                };
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Id = UidGenerator.GenerateUniqueId("U").ToString(),
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PasswordHash = hashedPassword
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return new ServiceResponse
            {
                Success = true,
                Message = "User registered successfully."
            };
        }

        // -------------------------------------------------------
        // Login
        // -------------------------------------------------------
        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return new LoginResponseDto
                {
                    Success = false,
                    Message = "Invalid credentials."
                };
            }

            var token = _jwt.GenerateToken(user);

            return new LoginResponseDto
            {
                Success = true,
                Token = token,
                Email = user.Email,
                UserId = user.Id
            };
        }

        // -------------------------------------------------------
        // Get Authenticated User Profile
        // -------------------------------------------------------
        public async Task<UserProfileDto?> GetUserProfileAsync(string userId)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return null;

            return new UserProfileDto
            {
                UserId = user.Id,
                Email = user.Email
            };
        }

    }
}
