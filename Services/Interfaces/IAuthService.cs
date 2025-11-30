using SNSCakeBakery_Service.DTO.Login;
using SNSCakeBakery_Service.DTO.Register;
using SNSCakeBakery_Service.DTOs.Auth.SNSCakeBakery_Service.DTOs.Auth;

namespace SNSCakeBakery_Service.Services.Interfaces
{
    public interface IAuthService
    {
        AuthDto? Register(RegisterRequestDto request);
        AuthDto? Login(LoginRequestDto request);
    }
}
