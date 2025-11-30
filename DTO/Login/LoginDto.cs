namespace SNSCakeBakery_Service.DTO.Login
{
    public class LoginRequestDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class LoginResponseDto
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string? Email { get; set; }
        public string? UserId { get; set; }
        public string? Message { get; set; }
    }


}
