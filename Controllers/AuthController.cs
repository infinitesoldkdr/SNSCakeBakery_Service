using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SNSCakeBakery_Service.DTO.Login;
using SNSCakeBakery_Service.DTO.Register;
using SNSCakeBakery_Service.DTOs.Auth;
using SNSCakeBakery_Service.Models;
using SNSCakeBakery_Service.Services.Interfaces;
using System.Security.Claims;

namespace SNSCakeBakery.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // ----------------------------------------------------
        // GET: /api/user/me
        // ----------------------------------------------------
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized(new { authenticated = false });

            var user = await _userService.GetUserProfileAsync(userId);

            if (user == null)
                return Unauthorized(new { authenticated = false });

            return Ok(new
            {
                authenticated = true,
                id = user.UserId,
                email = user.Email
            });
        }

        // ----------------------------------------------------
        // POST: /api/user/register
        // ----------------------------------------------------
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest(new { success = false, message = "Email and password required." });

            //var user = await _userService.RegisterAsync(dto.Password, dto.Email);
            var user = _userService.RegisterAsync(new RegisterRequestDto
            {
                Email = dto.Email,
                Password = dto.Password
            });

            if (user == null)
                return BadRequest(new { success = false, message = "Email already registered." });

            return Ok(new { success = true, userId = user.Id });
        }

        // ----------------------------------------------------
        // POST: /api/user/login
        // ----------------------------------------------------
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            //var token = await _userService.LoginAsync(dto.Email, dto.Password);
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
