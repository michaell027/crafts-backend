using crafts_api.models;
using crafts_api.services;
using Microsoft.AspNetCore.Mvc;

namespace crafts_api.controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    
    public AuthController(AuthService authService) =>
        _authService = authService;
    
    // REGISTER
    [HttpPost("register")]
    public IActionResult Register(UserDto userDto)
    {
        var user = new User
        {
            Username = userDto.Username,
            Password = userDto.Password,
            Email = userDto.Email,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        
        _authService.Register(user);
        return Ok("User registered successfully");
    }
    
    // LOGIN
    [HttpPost("login")]
    public IActionResult Login(LoginRequest loginRequest)
    {
        var loggedInUser = _authService.Login(loginRequest);
        var token = _authService.CreateToken(loggedInUser);
        return loggedInUser.Id != 0 ? Ok(token) : NotFound("Invalid username or password");
    }

}