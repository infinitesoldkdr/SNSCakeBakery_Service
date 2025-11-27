using Microsoft.AspNetCore.Mvc;
using SNSCakeBakery_Service.Services.Interfaces;
using SNSCakeBakery_Service.DTOs.Auth;

namespace SNSCakeBakery_Service.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth)
    {
        _auth = auth;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var result = await _auth.Register(request);
        if (result == null) return BadRequest("User already exists.");
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _auth.Login(request);
        if (result == null) return Unauthorized("Invalid credentials.");
        return Ok(result);
    }
}
