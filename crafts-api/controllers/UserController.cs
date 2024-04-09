using crafts_api.Entities.Dto;
using crafts_api.Interfaces;
using crafts_api.models;
using crafts_api.services;
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
    [HttpGet("get-user")]
    public async Task<UserDto> GetUserById(Guid id)
    {
        return await _userService.GetUser(id);
    }

}