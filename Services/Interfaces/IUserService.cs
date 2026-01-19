using SNSCakeBakery_Service.DTO.Login;
using SNSCakeBakery_Service.DTO.Register;
using SNSCakeBakery_Service.DTO.Service;
using SNSCakeBakery_Service.DTO.User;
using SNSCakeBakery_Service.DTOs.Auth;
using SNSCakeBakery_Service.DTOs.Users;

namespace SNSCakeBakery_Service.Services.Interfaces
{
    public interface IUserService
    {
        Task<ServiceResponse> RegisterAsync(RegisterRequestDto request);
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
        Task<UserProfileDto?> GetUserProfileAsync(string userId);
        Task<ServiceResponse> SyncFirebaseUserAsync(string firebaseUid, UserSyncDto dto);
    }
}
