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
using System.Reflection;
using System.Security.Claims;

namespace crafts_api.services;

public class AuthService(DatabaseContext databaseContext, IConfiguration configuration, UserManager<IdentityUser> userManager) : IAuthService
{
    private readonly DatabaseContext _databaseContext = databaseContext;
    private readonly TokenFunctions tokenFunctions = new(configuration);
    private readonly UserManager<IdentityUser> _userManager = userManager;

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
            Email = registerRequest.Email,
            AccessFailedCount = 5
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
    public async Task<LoggedUser> Login(LoginRequest loginRequest)
    {
        var identityUser = await _userManager.FindByEmailAsync(loginRequest.Email);
        if (identityUser is null)
            throw CreateUserNotFoundException();

        var result = await _userManager.CheckPasswordAsync(identityUser, loginRequest.Password);
        if (!result)
            throw CreateInvalidPasswordException();

        return loginRequest.Role switch
        {
            Role.User => await GetUser(identityUser),
            Role.Crafter => await GetCraftsman(identityUser),
            Role.Admin => await GetAdmin(identityUser),
            _ => throw CreateUserNotFoundException()
        };
    }
    public async Task<LoggedUser> RefreshToken(RefreshTokenRequest refreshTokenRequest)
    {
        var principal = tokenFunctions.GetPrincipalFromExpiredToken(refreshTokenRequest.Token);
        
        if (principal is null)
            throw new DefaultException { StatusCode = HttpStatusCode.BadRequest, ErrorCode = 400, Message = "Invalid token" };

        var publicId = principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

        var user = await _databaseContext.Users.FirstOrDefaultAsync(user => user.PublicId.ToString() == publicId) ?? throw CreateUserNotFoundException();

        var userRefreshToken = await _databaseContext.RefreshTokens.FirstOrDefaultAsync(token => token.UserIdentityId == user.IdentityId) ?? throw new DefaultException { StatusCode = HttpStatusCode.BadRequest, ErrorCode = 400, Message = "Token not found" };

        if (userRefreshToken.Token != refreshTokenRequest.RefreshToken)
            throw new DefaultException { StatusCode = HttpStatusCode.BadRequest, ErrorCode = 400, Message = "Invalid token" };

        if (userRefreshToken.Expires < DateTime.Now)
            throw new DefaultException { StatusCode = HttpStatusCode.BadRequest, ErrorCode = 400, Message = "Token expired" };

        var token = tokenFunctions.CreateToken(new LoggedUserDto
        {
            PublicId = user.PublicId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.Username,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        });

        var loggedInUser = new LoggedUser
        {
            User = new LoggedUserDto
            {
                PublicId = user.PublicId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.Username,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            },
            Token = token,
            RefreshToken = userRefreshToken.Token
        };

        return loggedInUser;
    }
    public async Task UpdateUserProfile(UpdateUserProfileRequest updateProfileRequest, string token)
    {
        var publicId = tokenFunctions.GetClaim(token, "nameid");
        var role = tokenFunctions.GetClaim(token, "role");

        if (publicId == null || role != Role.User.ToString())
        {
            throw new DefaultException
            {
                StatusCode = HttpStatusCode.BadRequest,
                ErrorCode = 400,
                Message = "Unauthorized"
            };
        }

        var user = _databaseContext.Users.FirstOrDefault(user => user.PublicId.ToString() == publicId);

        if (user == null)
        {
            throw new DefaultException
            {
                StatusCode = HttpStatusCode.BadRequest,
                ErrorCode = 400,
                Message = "User not found"
            };
        }

        if (updateProfileRequest.Email != user.Email && EmailExists(updateProfileRequest.Email))
        {
            throw new DefaultException
            {
                StatusCode = HttpStatusCode.BadRequest,
                ErrorCode = 400,
                Message = "Email already exists"
            };
        }

        var identityUser = await _userManager.FindByIdAsync(user.IdentityId);

        if (identityUser == null)
        {
            throw new DefaultException
            {
                StatusCode = HttpStatusCode.BadRequest,
                ErrorCode = 400,
                Message = "User not found"
            };
        }

        if (updateProfileRequest.IsEmpty())
        {
            throw new DefaultException
            {
                StatusCode = HttpStatusCode.BadRequest,
                ErrorCode = 400,
                Message = "Nothing to update"
            };
        }

        updateProfileRequest.UpdateUserFields(user, identityUser);
        user.UpdatedAt = DateTime.Now;
        await _databaseContext.SaveChangesAsync();
    }
    public async Task Revoke(string Token)
    {
        var publicId = tokenFunctions.GetClaim(Token, "nameid") ?? throw new DefaultException { StatusCode = HttpStatusCode.BadRequest, ErrorCode = 400, Message = "Invalid token" };

        var user = await _databaseContext.Users.FirstOrDefaultAsync(user => user.PublicId.ToString() == publicId) ?? throw CreateUserNotFoundException();

        var userRefreshToken = await _databaseContext.RefreshTokens.FirstOrDefaultAsync(token => token.UserIdentityId == user.IdentityId) ?? throw new DefaultException { StatusCode = HttpStatusCode.BadRequest, ErrorCode = 400, Message = "Token not found" };

        _databaseContext.RefreshTokens.Remove(userRefreshToken);
        await _databaseContext.SaveChangesAsync();
    }


    private async Task<LoggedUser> GetCraftsman(IdentityUser identityUser) =>
    await GetLoggedUserDto(identityUser, Role.Crafter);
    private async Task<LoggedUser> GetAdmin(IdentityUser identityUser)
    {
        throw new NotImplementedException();
    }
    private async Task<LoggedUser> GetUser(IdentityUser identityUser) =>
    await GetLoggedUserDto(identityUser, Role.User);
    private async Task<LoggedUser> GetLoggedUserDto(IdentityUser identityUser, Role role)
    {
        switch (role)
        {
            case Role.User:
                var user = await _databaseContext.Users.FirstOrDefaultAsync(user => user.IdentityId == identityUser.Id) ?? throw CreateUserNotFoundException();
                var userDto = new LoggedUserDto
                {
                    PublicId = user.PublicId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.Username,
                    Email = user.Email,
                    Role = user.Role,
                    CreatedAt = user.CreatedAt,
                };
                var (userToken, userRefreshToken) = CreateTokens(userDto);
                await StoreRefreshToken(user.IdentityId, userRefreshToken);
                return new LoggedUser { User = userDto, Token = userToken, RefreshToken = userRefreshToken };
            case Role.Crafter:
                var craftsman = _databaseContext.Crafters.FirstOrDefault(craftsman => craftsman.IdentityId == identityUser.Id) ?? throw CreateUserNotFoundException();
                var craftsmanDto = new LoggedUserDto
                {
                    PublicId = craftsman.PublicId,
                    FirstName = craftsman.FirstName,
                    LastName = craftsman.LastName,
                    UserName = craftsman.Username,
                    Email = craftsman.Email,
                    Role = craftsman.Role,
                    CreatedAt = craftsman.CreatedAt,
                };
                var (craftsmanToken, craftsmanRefreshToken) = CreateTokens(craftsmanDto);
                return new LoggedUser { User = craftsmanDto, Token = craftsmanToken, RefreshToken = craftsmanRefreshToken };
            default:
                throw new ArgumentException("Invalid role");
        }
    }
    private async Task StoreRefreshToken(string identityId, string userRefreshToken)
    {
        var refreshToken = new RefreshToken
        {
            Token = userRefreshToken,
            Expires = DateTime.Now.AddDays(7),
            UserIdentityId = identityId
        };

        await _databaseContext.RefreshTokens.AddAsync(refreshToken);
        await _databaseContext.SaveChangesAsync();
    }
    private (string, string) CreateTokens(LoggedUserDto userDto)
    {
        string token = tokenFunctions.CreateToken(userDto);
        string refreshTokenString = tokenFunctions.GenerateRefreshTokenAsync();
        return (token, refreshTokenString);
    }





    private Boolean EmailExists(string email)
    {
        return _databaseContext.Users.Any(user => user.Email == email);
    }
    private Boolean PasswordsMatch(string password, string passwordConfirmation)
    {
        return password == passwordConfirmation;
    }



    private DefaultException CreateUserNotFoundException() =>
    new DefaultException { StatusCode = HttpStatusCode.BadRequest, ErrorCode = 400, Message = "User not found" };
    private DefaultException CreateInvalidPasswordException() =>
    new DefaultException { StatusCode = HttpStatusCode.BadRequest, ErrorCode = 400, Message = "Invalid password" };


}

public static class UpdateUserProfileRequestExtensions
{
    public static bool IsEmpty(this UpdateUserProfileRequest updateProfileRequest)
    {
        return string.IsNullOrWhiteSpace(updateProfileRequest.Username) &&
           string.IsNullOrWhiteSpace(updateProfileRequest.Email) &&
           string.IsNullOrWhiteSpace(updateProfileRequest.FirstName) &&
           string.IsNullOrWhiteSpace(updateProfileRequest.LastName) &&
           string.IsNullOrWhiteSpace(updateProfileRequest.ProfilePicture) &&
           string.IsNullOrWhiteSpace(updateProfileRequest.Country) &&
           string.IsNullOrWhiteSpace(updateProfileRequest.City) &&
           string.IsNullOrWhiteSpace(updateProfileRequest.Address) &&
           string.IsNullOrWhiteSpace(updateProfileRequest.Street) &&
           string.IsNullOrWhiteSpace(updateProfileRequest.Number) &&
           string.IsNullOrWhiteSpace(updateProfileRequest.PostalCode) &&
           string.IsNullOrWhiteSpace(updateProfileRequest.PhoneNumber);
    }

    public static void UpdateUserFields (this UpdateUserProfileRequest updateProfileRequest, User user, IdentityUser identityUser)
    {
        PropertyInfo[] properties = typeof(UpdateUserProfileRequest).GetProperties();

        foreach (PropertyInfo property in properties)
        {
            string? value = property.GetValue(updateProfileRequest)?.ToString();
            if (!string.IsNullOrWhiteSpace(value))
            {
                PropertyInfo? userProperty = typeof(User).GetProperty(property.Name);
                if (userProperty != null)
                {
                    userProperty.SetValue(user, value);
                }

                PropertyInfo? identityUserProperty = typeof(IdentityUser).GetProperty(property.Name);
                if (identityUserProperty != null)
                {
                    identityUserProperty.SetValue(identityUser, value);
                }
            }
        }
    }
}
