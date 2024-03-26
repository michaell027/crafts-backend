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
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace crafts_api.services;

public class AuthService : IAuthService
{
    private readonly DatabaseContext _databaseContext;
    private readonly TokenFunctions tokenFunctions;
    private readonly UserManager<IdentityUser> _userManager;

    public AuthService(DatabaseContext databaseContext, IConfiguration configuration, UserManager<IdentityUser> userManager)
    {
        _databaseContext = databaseContext;
        tokenFunctions = new TokenFunctions(configuration);
        _userManager = userManager;
    }

    public async Task CraftsmanRegister(RegisterCraftsmanRequest registerCraftsmanRequest)
    {

        if (!PasswordsMatch(registerCraftsmanRequest.Password, registerCraftsmanRequest.PasswordConfirmation))
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
            UserName = registerCraftsmanRequest.Username,
            Email = registerCraftsmanRequest.Email
        };

        var craftsmanProfile = new CraftsmanProfile
        {
            Bio = registerCraftsmanRequest.Bio,
            PhoneNumber = registerCraftsmanRequest.PhoneNumber,
            Address = registerCraftsmanRequest.Address,
            City = registerCraftsmanRequest.City,
            Country = registerCraftsmanRequest.Country,
            Street = registerCraftsmanRequest.Street,
            Number = registerCraftsmanRequest.Number,
            PostalCode = registerCraftsmanRequest.PostalCode,
            ProfilePicture = registerCraftsmanRequest.ProfilePicture
        };

        var craftsman = new Craftsman
        {
            PublicId = Guid.NewGuid(),
            IdentityId = identityUser.Id,
            Username = registerCraftsmanRequest.Username,
            FirstName = registerCraftsmanRequest.FirstName,
            LastName = registerCraftsmanRequest.LastName,
            Email = registerCraftsmanRequest.Email,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            Role = Role.Crafter,
            CraftsmanProfile = craftsmanProfile
        };

        var result = await _userManager.CreateAsync(identityUser, registerCraftsmanRequest.Password);

        if (!result.Succeeded)
        {
            throw new DefaultException
            {
                StatusCode = HttpStatusCode.InternalServerError,
                ErrorCode = 500,
                Message = result.Errors.First().Description
            };
        }

        await _databaseContext.Crafters.AddAsync(craftsman);
        await _databaseContext.SaveChangesAsync();

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

        if (loginRequest.Role == Role.User)
        {
            return await GetUser(identityUser);
        }
        else if (loginRequest.Role == Role.Crafter)
        {
            return await GetCraftsman(identityUser);
        }
        else if (loginRequest.Role == Role.Admin)
        {
            return await GetAdmin(identityUser);
        }
        else
        {
            throw new DefaultException
            {
                StatusCode = HttpStatusCode.BadRequest,
                ErrorCode = 400,
                Message = "User not found"
            };
        }
    }

    private async Task<LoggedUser> GetAdmin(IdentityUser identityUser)
    {
        throw new NotImplementedException();
    }

    private async Task<LoggedUser> GetCraftsman(IdentityUser identityUser)
    {
        Craftsman? craftsman = await _databaseContext.Crafters.FirstOrDefaultAsync(craftsman => craftsman.IdentityId == identityUser.Id);

        if (craftsman == null)
        {
            throw new DefaultException
            {
                StatusCode = HttpStatusCode.BadRequest,
                ErrorCode = 400,
                Message = "User not found"
            };
        }

        LoggedUserDto userDto = new LoggedUserDto
        {
            PublicId = craftsman.PublicId,
            FirstName = craftsman.FirstName,
            LastName = craftsman.LastName,
            UserName = craftsman.Username,
            Email = craftsman.Email,
            Role = craftsman.Role,
            CreatedAt = craftsman.CreatedAt,
        };

        string token = tokenFunctions.CreateToken(userDto);
        string refreshTokenString = tokenFunctions.GenerateRefreshTokenAsync();

        var refreshToken = new RefreshToken
        {
            Token = refreshTokenString,
            Expires = DateTime.Now.AddDays(7),
            UserIdentityId = craftsman.IdentityId
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


    public async Task UserRegister(RegisterUserRequest registerRequest)
    {
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

        var userProfile = new UserProfile
        {
            ProfilePicture = registerRequest.ProfilePicture,
            Country = registerRequest.Country,
            City = registerRequest.City,
            Address = registerRequest.Address,
            Street = registerRequest.Street,
            Number = registerRequest.Number,
            PostalCode = registerRequest.PostalCode,
            PhoneNumber = registerRequest.PhoneNumber
        };

        var user = new User
        {
            PublicId = Guid.NewGuid(),
            IdentityId = identityUser.Id,
            Username = registerRequest.Username,
            FirstName = registerRequest.FirstName,
            LastName = registerRequest.LastName,
            Email = registerRequest.Email,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            Role = Role.User,
            UserProfile = userProfile
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

    private Boolean EmailExists(string email)
    {
        return _databaseContext.Users.Any(user => user.Email == email);
    }

    private Boolean PasswordsMatch(string password, string passwordConfirmation)
    {
        return password == passwordConfirmation;
    }

    private async Task<LoggedUser> GetUser(IdentityUser identityUser)
    {
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

        LoggedUserDto userDto = new LoggedUserDto
        {
            PublicId = user.PublicId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.Username,
            Email = user.Email,
            Role = user.Role,
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
}