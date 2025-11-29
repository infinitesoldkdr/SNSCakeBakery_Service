using SNSCakeBakery_Service.DTOs.Auth;

namespace SNSCakeBakery_Service.Services.Interfaces;

public interface IAuthService
{
    Task<AuthDto?> Login(LoginRequest request);
    Task<AuthDto?> Register(RegisterRequest request);
}
