using Microsoft.AspNetCore.Mvc;
using MathTutor.Core.Models.Auth;
using MathTutor.Application.Interfaces;

namespace MathTutor.API.Controllers;

public class AuthController : BaseApiController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="model">The registration details</param>
    /// <returns>Authentication result with token</returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
 
        var result = await _authService.RegisterAsync(model);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    /// <summary>
    /// Authenticate a user
    /// </summary>
    /// <param name="model">The login credentials</param>
    /// <returns>Authentication result with token</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var result = await _authService.LoginAsync(model);

        if (result.Success)
            return Ok(result);

        return Unauthorized(result);
    }

    /// <summary>
    /// Refresh the authentication token
    /// </summary>
    /// <param name="token">The refresh token</param>
    /// <returns>New authentication token</returns>
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] string token)
    {
        var result = await _authService.RefreshTokenAsync(token);

        if (result.Success)
            return Ok(result);

        return Unauthorized(result);
    }
}