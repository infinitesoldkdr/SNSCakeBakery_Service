using Microsoft.EntityFrameworkCore;
using SNSCakeBakery_Service.Data;
using SNSCakeBakery_Service.DTO.Login;
using SNSCakeBakery_Service.DTO.Register;
using SNSCakeBakery_Service.DTOs.Auth;
using SNSCakeBakery_Service.DTOs.Auth.SNSCakeBakery_Service.DTOs.Auth;
using SNSCakeBakery_Service.Models;
using SNSCakeBakery_Service.Services.Helpers;
using SNSCakeBakery_Service.Services.Interfaces;

namespace SNSCakeBakery_Service.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly JwtTokenGenerator _jwt;

        public AuthService(AppDbContext db, JwtTokenGenerator jwt)
        {
            _db = db;
            _jwt = jwt;
        }

        public AuthDto? Register(RegisterRequestDto request)
        {
            if (_db.Users.Any(x => x.Email == request.Email))
                return null;

            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Email = request.Email,
                //FullName = request.FullName,
                PasswordHash = PasswordHasher.Hash(request.Password)
            };

            _db.Users.Add(user);
            _db.SaveChanges();

            return new AuthDto
            {
                Email = user.Email,
                FullName = user.FullName,
                Token = _jwt.GenerateToken(user)
            };
        }

        public AuthDto? Login(LoginRequestDto request)
        {
            var user = _db.Users.SingleOrDefault(x => x.Email == request.Email);
            if (user == null) return null;

            if (!PasswordHasher.Verify(request.Password, user.PasswordHash))
                return null;

            return new AuthDto
            {
                Email = user.Email,
                FullName = user.FullName,
                Token = _jwt.GenerateToken(user)
            };
        }
    }
}
