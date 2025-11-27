using SNSCakeBakery_Service.DTOs.Auth;

namespace SNSCakeBakery_Service.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponse?> Login(LoginRequest request);
    Task<AuthResponse?> Register(RegisterRequest request);
}
