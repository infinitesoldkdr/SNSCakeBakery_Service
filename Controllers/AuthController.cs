using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SNSCakeBakery_Service.DTO.Login;
using SNSCakeBakery_Service.DTO.Register;
using SNSCakeBakery_Service.DTOs.Auth;
using SNSCakeBakery_Service.Models;
using SNSCakeBakery_Service.Services.Interfaces;
using System.Security.Claims;

namespace SNSCakeBakery_Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        // ----------------------------------------------------
        // POST: /api/user/login
        // ----------------------------------------------------
        [HttpPost("authlogin")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var token = _userService.LoginAsync(new LoginRequestDto
            {
                Email = dto.Email,
                Password = dto.Password
            });

            if (token == null)
                return Unauthorized(new { success = false, message = "Invalid credentials." });

            return Ok(new
            {
                success = true,
                token = token
            });
        }
    }


}
