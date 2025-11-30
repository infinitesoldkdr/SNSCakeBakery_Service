using BCrypt.Net;

namespace SNSCakeBakery_Service.Services.Helpers
{
    public static class PasswordHasher
    {
        public static string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool Verify(string password, string hashed)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashed);
        }
    }
}
