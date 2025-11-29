namespace SNSCakeBakery_Service.DTOs.Auth
{
    // ---------------------------------------------
    // User Registration Request
    // ---------------------------------------------
    public class RegisterRequestDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
    }

    // ---------------------------------------------
    // User Login Request
    // ---------------------------------------------
    public class LoginRequestDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    // ---------------------------------------------
    // Auth Response returned after login/registration
    // ---------------------------------------------
    public class AuthResponseDto
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }

        // Optional: return name for UI display
        public string FullName { get; set; }
    }

    // ---------------------------------------------
    // DTO used when returning user info
    // ---------------------------------------------
    public class UserProfileDto
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
    }
}
