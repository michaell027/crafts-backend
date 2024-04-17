using crafts_api.Entities.Dto;
using crafts_api.Entities.Models;
using crafts_api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace crafts_api.controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService) =>
        _userService = userService;

    // GET USERS
//    [HttpGet]
//    public List<User> GetUsers()
//    {
//        return _userService.GetAllUsers();
//    }
    
    // GET USER BY ID
    [Authorize]
    [HttpGet("get-user")]
    public async Task<UserDto> GetUserById(Guid id)
    {
        var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        return await _userService.GetUser(id, token);
    }
    
    // UPDATE USER PROFILE
    [Authorize]
    [HttpPost("update-user-profile")]
    public async Task<IActionResult> UpdateUserProfile(UpdateUserProfileRequest updateProfileRequest)
    {
        var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        await _userService.UpdateUserProfile(updateProfileRequest, token);
        return Ok();
    }

}