//using crafts_api.models;
//using crafts_api.services;
//using Microsoft.AspNetCore.Mvc;

//namespace crafts_api.controllers;

//[Route("api/[controller]")]
//[ApiController]
//public class UserController : ControllerBase
//{
//    private readonly UserService _userService;

//    public UserController(UserService userService) =>
//        _userService = userService;

//    // GET USERS
//    [HttpGet]
//    public List<User> GetUsers()
//    {
//        return _userService.GetAllUsers();
//    }
    
//    // GET USER BY ID
//    [HttpGet("{id}")]
//    public User GetUserById(int id)
//    {
//        return _userService.GetUserById(id);
//    }

//}