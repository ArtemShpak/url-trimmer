using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.Dto;
using UrlShortener.Application.Interfaces;
using UrlShortener.Application.Interfaces.Services;

namespace UrlShortener.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : Controller
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
    {
        var result = await authService.LoginAsync(loginRequest);
        if (result.IsFailure) return Unauthorized(new { message = result.Error });

        return Ok(result);
    }

    [HttpPost("registration")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] LoginRequestDto loginRequest)
    {
        var result = await authService.RegisterAsync(loginRequest);
        if (result.IsFailure) return BadRequest(new { message = result.Error });

        return Ok(result);
    }

    [HttpGet("users")]
    [Authorize]
    public async Task<IActionResult> GetUsers()
    {
        var users = await authService.GetAllUsersAsync();
        return Ok(users.Value);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var result = await authService.DeleteUserByIdAsync(id);
        if (result.IsFailure) return BadRequest(new { message = result.Error });

        return NoContent();
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var result = await authService.GetCurrentUserAsync();
        if (result.IsFailure) return BadRequest(new { message = result.Error });

        return Ok(result.Value);
    }
    
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await authService.LogoutAsync();
        return Ok(new { message = "Logged out successfully" });
    }
}
