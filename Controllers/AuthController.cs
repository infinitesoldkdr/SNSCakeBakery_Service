using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SNSCakeBakery_Service.DTO.Login;
using SNSCakeBakery_Service.DTOs.Auth;
using SNSCakeBakery_Service.DTOs.Users; // Path for UserSyncDto
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

        /// <summary>
        /// Syncs the Firebase authenticated user with our Oracle Database.
        /// Uses the JWT Claim to identify the user safely.
        /// </summary>
        [Authorize]
        [HttpPost("sync")]
        public async Task<IActionResult> Sync([FromBody] UserSyncDto dto)
        {
            // Extract the UID from the Firebase JWT (NameIdentifier claim)
            var firebaseUid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(firebaseUid))
            {
                return Unauthorized(new { message = "Invalid token: Firebase UID missing." });
            }

            // The Service layer handles the 'Upsert' logic (Update or Insert)
            var result = await _userService.SyncFirebaseUserAsync(firebaseUid, dto);

            if (result == null)
                return BadRequest(new { message = "User synchronization failed." });

            return Ok(new { success = true, user = result });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            // FIX: Always await the task properly; avoid .Result
            var authResult = await _userService.LoginAsync(new LoginRequestDto
            {
                Email = dto.Email,
                Password = dto.Password
            });

            if (authResult?.Token == null)
                return Unauthorized(new { success = false, message = "Invalid credentials." });

            return Ok(new { success = true, token = authResult.Token });
        }
    }
}