using crafts_api.context;
using crafts_api.Entities.Domain;
using crafts_api.Entities.Enum;
using crafts_api.Entities.Models;
using crafts_api.exceptions;
using crafts_api.interfaces;
using crafts_api.utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;
using crafts_api.Entities.Dto;

namespace crafts_api.services;

public class AuthService(DatabaseContext databaseContext, IConfiguration configuration, UserManager<IdentityUser> userManager) : IAuthService
{
    private readonly TokenFunctions _tokenFunctions = new(configuration);

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

        var result = await userManager.CreateAsync(identityUser, registerCraftsmanRequest.Password);

        if (!result.Succeeded)
        {
            throw new DefaultException
            {
                StatusCode = HttpStatusCode.InternalServerError,
                ErrorCode = 500,
                Message = result.Errors.First().Description
            };
        }

        await databaseContext.Crafters.AddAsync(craftsman);
        await databaseContext.SaveChangesAsync();

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


        var result = await userManager.CreateAsync(identityUser, registerRequest.Password);


        if (!result.Succeeded)
        {
            throw new DefaultException
            {
                StatusCode = HttpStatusCode.InternalServerError,
                ErrorCode = 500,
                Message = result.Errors.First().Description
            };
        }

        await databaseContext.Users.AddAsync(user);
        await databaseContext.SaveChangesAsync();
    }
    public async Task<LoggedUser> Login(LoginRequest loginRequest)
    {
        var identityUser = await userManager.FindByEmailAsync(loginRequest.Email);
        if (identityUser is null)
            throw CreateUserNotFoundException();

        var result = await userManager.CheckPasswordAsync(identityUser, loginRequest.Password);
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
        var principal = _tokenFunctions.GetPrincipalFromExpiredToken(refreshTokenRequest.Token);
        
        if (principal is null)
            throw new DefaultException { StatusCode = HttpStatusCode.BadRequest, ErrorCode = 400, Message = "Invalid token" };

        var publicId = principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

        var user = await databaseContext.Users.FirstOrDefaultAsync(user => user.PublicId.ToString() == publicId) ?? throw CreateUserNotFoundException();

        var userRefreshToken = await databaseContext.RefreshTokens.FirstOrDefaultAsync(token => token.UserIdentityId == user.IdentityId) ?? throw new DefaultException { StatusCode = HttpStatusCode.BadRequest, ErrorCode = 400, Message = "Token not found" };

        if (userRefreshToken.Token != refreshTokenRequest.RefreshToken)
            throw new DefaultException { StatusCode = HttpStatusCode.BadRequest, ErrorCode = 400, Message = "Invalid token" };

        if (userRefreshToken.Expires < DateTime.Now)
            throw new DefaultException { StatusCode = HttpStatusCode.BadRequest, ErrorCode = 400, Message = "Token expired" };

        var token = _tokenFunctions.CreateToken(new LoggedUserDto
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
    public async Task Revoke(string token)
    {
        var publicId = _tokenFunctions.GetClaim(token, "nameid") ?? throw new DefaultException { StatusCode = HttpStatusCode.BadRequest, ErrorCode = 400, Message = "Invalid token" };

        var user = await databaseContext.Users.FirstOrDefaultAsync(user => user.PublicId.ToString() == publicId) ?? throw CreateUserNotFoundException();

        var userRefreshToken = await databaseContext.RefreshTokens.FirstOrDefaultAsync(t => t.UserIdentityId == user.IdentityId) ?? throw new DefaultException { StatusCode = HttpStatusCode.BadRequest, ErrorCode = 400, Message = "Token not found" };

        databaseContext.RefreshTokens.Remove(userRefreshToken);
        await databaseContext.SaveChangesAsync();
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
                var user = await databaseContext.Users.FirstOrDefaultAsync(user => user.IdentityId == identityUser.Id) ?? throw CreateUserNotFoundException();
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
                var craftsman = databaseContext.Crafters.FirstOrDefault(craftsman => craftsman.IdentityId == identityUser.Id) ?? throw CreateUserNotFoundException();
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
            case Role.Admin:
                throw new NotImplementedException();
            default:
                throw new ArgumentException("Invalid role");
        }
    }
    private async Task StoreRefreshToken(string identityId, string userRefreshToken)
    {
        if (await databaseContext.RefreshTokens.AnyAsync(token => token.UserIdentityId == identityId))
        {
            var refreshTokenFromDb = await databaseContext.RefreshTokens.FirstOrDefaultAsync(token => token.UserIdentityId == identityId) ?? throw new DefaultException { StatusCode = HttpStatusCode.BadRequest, ErrorCode = 400, Message = "Token not found" };
            refreshTokenFromDb.Token = userRefreshToken;
            refreshTokenFromDb.Expires = DateTime.Now.AddDays(7);
            await databaseContext.SaveChangesAsync();
            return;
        }
        
        var refreshToken = new RefreshToken
        {
            Token = userRefreshToken,
            Expires = DateTime.Now.AddDays(7),
            UserIdentityId = identityId
        };

        await databaseContext.RefreshTokens.AddAsync(refreshToken);
        await databaseContext.SaveChangesAsync();
    }
    private (string, string) CreateTokens(LoggedUserDto userDto)
    {
        string token = _tokenFunctions.CreateToken(userDto);
        string refreshTokenString = _tokenFunctions.GenerateRefreshTokenAsync();
        return (token, refreshTokenString);
    }


    private static bool PasswordsMatch(string password, string passwordConfirmation)
    {
        return password == passwordConfirmation;
    }



    private static DefaultException CreateUserNotFoundException() =>
    new() { StatusCode = HttpStatusCode.BadRequest, ErrorCode = 400, Message = "User not found" };
    private static DefaultException CreateInvalidPasswordException() =>
    new() { StatusCode = HttpStatusCode.BadRequest, ErrorCode = 400, Message = "Invalid password" };


}