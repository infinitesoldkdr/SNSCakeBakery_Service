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
        // Use the Atomic registration method as the main entry point
        Task<ServiceResponse> RegisterUser(CreateUserDto request); 
    
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
        Task<UserProfileDto?> GetUserProfileAsync(string userId);
        Task<ServiceResponse> SyncFirebaseUserAsync(string firebaseUid, UserSyncDto dto);
        // You can keep or remove RegisterAsync depending on if you still use it
        Task<ServiceResponse> RegisterAsync(RegisterRequestDto request);
    }
}
