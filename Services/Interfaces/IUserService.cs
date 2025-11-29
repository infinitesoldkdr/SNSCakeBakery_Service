using SNSCakeBakery_Service.Models;

namespace SNSCakeBakery_Service.Services.Interfaces
{
    public interface IUserService
    {
        Task<User?> RegisterAsync( string password, string email);
        Task<string?> LoginAsync(string email, string password);
        Task<User?> GetUserByIdAsync(string id);
        Task<User?> GetUserByEmailAsync(string email);
    }
}
