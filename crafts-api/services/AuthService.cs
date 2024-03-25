using crafts_api.context;
using crafts_api.Entities.Domain;
using crafts_api.Entities.Enum;
using crafts_api.Entities.Models;
using crafts_api.exceptions;
using crafts_api.interfaces;
using crafts_api.models.domain;
using crafts_api.models.dto;
using crafts_api.models.models;
using crafts_api.utils;
using crafts_api.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace crafts_api.services;

public class AuthService : IAuthService
{
    private readonly DatabaseContext _databaseContext;
    private readonly PasswordFunctions passwordFunctions;
    private readonly TokenFunctions tokenFunctions;
    private readonly UserManager<IdentityUser> _userManager;

    public AuthService(DatabaseContext databaseContext, IConfiguration configuration, UserManager<IdentityUser> userManager)
    {
        _databaseContext = databaseContext;
        passwordFunctions = new PasswordFunctions();
        tokenFunctions = new TokenFunctions(configuration);
        _userManager = userManager;
    }

    public async Task<LoggedUser> Login(LoginRequest loginRequest)
    {
        var identityUser = await _userManager.FindByEmailAsync(loginRequest.Email);
        if (identityUser is null)
        {
            throw new DefaultException
            {
                StatusCode = HttpStatusCode.BadRequest,
                ErrorCode = 400,
                Message = "User not found"
            };
        }

        var result = await _userManager.CheckPasswordAsync(identityUser, loginRequest.Password);

        if (!result)
        {
            throw new DefaultException
            {
                StatusCode = HttpStatusCode.BadRequest,
                ErrorCode = 400,
                Message = "Invalid password"
            };
        }

        User? user = await _databaseContext.Users.FirstOrDefaultAsync(user => user.IdentityId == identityUser.Id);

        if (user == null)
        {
            throw new DefaultException
            {
                StatusCode = HttpStatusCode.BadRequest,
                ErrorCode = 400,
                Message = "User not found"
            };
        }

        UserDto userDto = new UserDto
        {
            PublicId = user.PublicId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
        };

        string token = tokenFunctions.CreateToken(userDto);
        string refreshTokenString = tokenFunctions.GenerateRefreshTokenAsync();

        var refreshToken = new RefreshToken
        {
            Token = refreshTokenString,
            Expires = DateTime.Now.AddDays(7),
            UserIdentityId = user.IdentityId
        };

        return new LoggedUser
        {
            User = userDto,
            Token = token
        };
    }

    public Task<string> RefreshToken(RefreshTokenRequest refreshTokenRequest)
    {
        var principal = tokenFunctions.GetPrincipalFromExpiredToken(refreshTokenRequest.Token);
        var username = principal.Identity?.Name;
        Console.WriteLine(principal);
        return null;
    }


    public async Task Register(RegisterUserRequest registerRequest, Role role)
    {
        Console.WriteLine(role.ToString());
        if (!PasswordsMatch(registerRequest.Password, registerRequest.PasswordConfirmation))
        {
            throw new DefaultException
            {
                StatusCode = HttpStatusCode.BadRequest,
                ErrorCode = 400,
                Message = "Passwords do not match"
            };
        }

        var identityUser = new IdentityUser
        {
            UserName = registerRequest.Username,
            Email = registerRequest.Email
        };

        User user = new User
        {
            PublicId = Guid.NewGuid(),
            IdentityId = identityUser.Id,
            Username = registerRequest.Username,
            FirstName = registerRequest.FirstName,
            LastName = registerRequest.LastName,
            Email = registerRequest.Email,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            Role = role
        };

        var result = await _userManager.CreateAsync(identityUser, registerRequest.Password);


        if (!result.Succeeded)
        {
            throw new DefaultException
            {
                StatusCode = HttpStatusCode.InternalServerError,
                ErrorCode = 500,
                Message = result.Errors.First().Description
            };
        }

        await _databaseContext.Users.AddAsync(user);
        await _databaseContext.SaveChangesAsync();
    }

    public void TestError()
    {
        throw new DefaultException
        {
            StatusCode = HttpStatusCode.InternalServerError,
            ErrorCode = 500,
            Message = "Test error"
        };
    }

    private Boolean EmailExists(string email)
    {
        return _databaseContext.Users.Any(user => user.Email == email);
    }

    private Boolean PasswordsMatch(string password, string passwordConfirmation)
    {
        return password == passwordConfirmation;
    }
}