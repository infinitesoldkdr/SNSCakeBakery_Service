namespace SNSCakeBakery_Service.DTOs.Auth
{

    // ---------------------------------------------
    // Auth Response returned after login/registration
    // ---------------------------------------------
    namespace SNSCakeBakery_Service.DTOs.Auth
    {
        public class AuthDto
        {
            public string Email { get; set; } = "";
            public string FirstName { get; set; } = "";
            public string LastName { get; set; } = "";
            public string Token { get; set; } = "";
        }
    }

    public class AuthResponseDto
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }

        // Optional: return name for UI display
        public string FirstName { get; set; }

        public string LastName { get; set; }
    }

    public class RegisterDto
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class LoginDto
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class MeDto
    {
        public string Email { get; set; }
        public string UserID {  get; set; }
    }
}
