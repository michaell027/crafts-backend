using crafts_api.Entities.Enum;
using crafts_api.Entities.Models;
using crafts_api.interfaces;
using crafts_api.models.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace crafts_api.controllers;

/// <summary>
/// Authentication controller
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    /// <summary>
    /// AuthController constructor
    /// </summary>
    /// <param name="authService"></param>
    public AuthController(IAuthService authService) =>
        _authService = authService;

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="registerRequest"></param>
    [HttpPost("register-user")]
    public async Task<IActionResult> UserRegister(RegisterUserRequest registerUserRequest)
    {
        await _authService.UserRegister(registerUserRequest);
        return Ok();
    }

    [HttpPost("register-craftsman")]
    public async Task<IActionResult> CraftsmanRegister(RegisterCraftsmanRequest registerCraftsmanRequest)
    {
        await _authService.CraftsmanRegister(registerCraftsmanRequest);
        return Ok();
    }

    /// <summary>
    /// Login a user and return a JWT token
    /// </summary>
    /// <param name="loginRequest"></param>
    /// <returns></returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest loginRequest)
    {
        LoggedUser loggedUser = await _authService.Login(loginRequest);
        return Ok(loggedUser);
    }

    [Authorize]
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequest refreshTokenRequest)
    {
        Console.WriteLine("Refresh token " + refreshTokenRequest.RefreshToken);
        return Ok();
    }
}