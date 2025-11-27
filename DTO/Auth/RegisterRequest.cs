namespace SNSCakeBakery_Service.DTOs.Auth;

public class RegisterRequest
{
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}
