using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SNSCakeBakery_Service.DTO.Login;
using SNSCakeBakery_Service.DTO.Register;
using SNSCakeBakery_Service.DTOs.Auth;
using SNSCakeBakery_Service.DTOs.Users;
using SNSCakeBakery_Service.Services.Interfaces;  // IUserService
using System.Security.Claims;
using System.Threading.Tasks;

namespace SNSCakeBakery_Service.Controllers
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

        // -------------------------------------------------------
        // POST: api/user/register
        // -------------------------------------------------------
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUserDto request)
        {
            var result = await _userService.RegisterUser(request);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(result);
        }

        // -------------------------------------------------------
        // POST: api/user/login
        // -------------------------------------------------------
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var result = await _userService.LoginAsync(request);

            if (!result.Success)
                return Unauthorized(new { message = result.Message });

            return Ok(result);
        }


        // ----------------------------------------------------
        // GET: /api/user/me
        // ----------------------------------------------------
        [HttpGet("GetMyProfile")]
        [Authorize]
        public async Task<IActionResult> Me()
        {
           var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) 
                 ?? User.FindFirstValue("sub");

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

    [HttpPost("sync")]
    [Authorize] // Critical: Ensures only valid Firebase users can sync
    public async Task<IActionResult> SyncUser([FromBody] UserSyncDto request)
    {
        // Extract UID from the Firebase JWT claims
        var firebaseUid = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(firebaseUid))
            return Unauthorized(new { message = "Firebase UID not found in token." });

        // Pass to service to handle the EF Core logic (Check if exists, then Create/Update)
        var result = await _userService.SyncFirebaseUserAsync(firebaseUid, request);

        if (!result.Success)
            return BadRequest(new { message = result.Message });

        return Ok(result);
    }
    }
}
