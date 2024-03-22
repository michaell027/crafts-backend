using crafts_api.context;
using crafts_api.Entities.Models;
using crafts_api.exceptions;
using crafts_api.interfaces;
using crafts_api.models.domain;
using crafts_api.models.dto;
using crafts_api.models.models;
using crafts_api.utils;
using crafts_api.Utils;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace crafts_api.services;

public class AuthService : IAuthService
{
    private readonly DatabaseContext _databaseContext;
    private readonly PasswordFunctions passwordFunctions;
    private readonly TokenFunctions tokenFunctions;

    public AuthService(DatabaseContext databaseContext, IConfiguration configuration)
    {
        _databaseContext = databaseContext;
        passwordFunctions = new PasswordFunctions();
        tokenFunctions = new TokenFunctions(configuration);
    }

    public async Task<LoggedUser> Login(LoginRequest loginRequest)
    {
        if (!EmailExists(loginRequest.Email))
        {
            throw new DefaultException
            {
                StatusCode = HttpStatusCode.BadRequest,
                ErrorCode = 400,
                Message = "Email does not exist"
            };
        }

        User user = await _databaseContext.Users.FirstOrDefaultAsync(user => user.Email == loginRequest.Email);

        Boolean passwordMatch = passwordFunctions.VerifyPassword(loginRequest.Password, user.Password);

        if (!passwordMatch)
        {
            throw new DefaultException
            {
                StatusCode = HttpStatusCode.BadRequest,
                ErrorCode = 400,
                Message = "Invalid password"
            };
        }

        UserDto userDto = new UserDto
        {
            PublicId = user.PublicId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
        };

        string token = tokenFunctions.CreateToken(userDto);

        return new LoggedUser
        {
            User = userDto,
            Token = token
        };
    }

    public async Task Register(RegisterRequest registerRequest)
    {
        if (EmailExists(registerRequest.Email))
        {
            throw new DefaultException
            {
                StatusCode = HttpStatusCode.BadRequest,
                ErrorCode = 400,
                Message = "Email already exists"
            };
        }

        Boolean passwordsMatch = registerRequest.Password == registerRequest.PasswordConfirmation;
        if (!passwordsMatch)
        {
            throw new DefaultException
            {
                StatusCode = HttpStatusCode.BadRequest,
                ErrorCode = 400,
                Message = "Passwords do not match"
            };
        }

        User user = new User
        {
            PublicId = Guid.NewGuid(),
            FirstName = registerRequest.FirstName,
            LastName = registerRequest.LastName,
            Password = passwordFunctions.HashPassword(registerRequest.Password),
            Email = registerRequest.Email,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

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
}