using System.Net;
using crafts_api.context;
using crafts_api.Entities.Dto;
using crafts_api.Entities.Enum;
using crafts_api.Entities.Models;
using crafts_api.Entities.Domain;
using crafts_api.exceptions;
using crafts_api.Interfaces;
using crafts_api.utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace crafts_api.services;

public class UserService(DatabaseContext databaseContext, IConfiguration configuration, UserManager<IdentityUser> userManager) : IUserService
{
    private readonly TokenFunctions _tokenFunctions = new(configuration);

    public async Task<UserDto> GetUser(Guid publicId, string token)
    {
        var role = _tokenFunctions.GetClaim(token, "role");
        
        if (role != Role.Crafter.ToString())
        {
            throw new DefaultException
            {
                StatusCode = HttpStatusCode.BadRequest,
                ErrorCode = 400,
                Message = "Unauthorized"
            };
        }
        
        if (publicId == Guid.Empty)
        {
            throw new DefaultException
            {
                StatusCode = HttpStatusCode.BadRequest,
                ErrorCode = 400,
                Message = "User public id is required"
            };
        }

        var user = await databaseContext.Users
            .Include(x => x.UserProfile)
            .FirstOrDefaultAsync(user => user.PublicId == publicId);

        if (user == null)
        {
            throw new DefaultException
            {
                StatusCode = HttpStatusCode.NotFound,
                ErrorCode = 404,
                Message = "User not found"
            };
        }
        
        if (user.Role != Role.User)
        {
            throw new DefaultException
            {
                StatusCode = HttpStatusCode.BadRequest,
                ErrorCode = 400,
                Message = "This is not a user"
            };
        }

        return new UserDto()
        {
            PublicId = user.PublicId,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Username = user.Username,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            Role = user.Role,
            ProfilePicture = user.UserProfile.ProfilePicture,
            Country = user.UserProfile.Country,
            City = user.UserProfile.City,
            Address = user.UserProfile.Address,
            Street = user.UserProfile.Street,
            Number = user.UserProfile.Number,
            PostalCode = user.UserProfile.PostalCode,
            PhoneNumber = user.UserProfile.PhoneNumber
        };
    }
    
    public async Task UpdateUserProfile(UpdateUserProfileRequest updateProfileRequest, string token)
    {
        var publicId = _tokenFunctions.GetClaim(token, "nameid");
        var role = _tokenFunctions.GetClaim(token, "role");

        if (publicId == null || role != Role.User.ToString())
        {
            throw new DefaultException
            {
                StatusCode = HttpStatusCode.BadRequest,
                ErrorCode = 400,
                Message = "Unauthorized"
            };
        }

        var user = databaseContext.Users.FirstOrDefault(user => user.PublicId.ToString() == publicId);

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

        var identityUser = await userManager.FindByIdAsync(user.IdentityId);

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
        await databaseContext.SaveChangesAsync();
    }

    private bool EmailExists(string email)
    {
        return databaseContext.Users.Any(user => user.Email == email);
    }

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
     
         public static void UpdateUserFields(this UpdateUserProfileRequest updateProfileRequest, User user, IdentityUser identityUser)
         {
             var properties = typeof(UpdateUserProfileRequest).GetProperties();
     
             foreach (var property in properties)
             {
                 var value = property.GetValue(updateProfileRequest)?.ToString() ?? string.Empty;
     
                 var userProperty = typeof(User).GetProperty(property.Name);
                 if (userProperty != null && !string.IsNullOrWhiteSpace(value))
                 {
                     userProperty.SetValue(user, value);
                     
                        if (property.Name == "Email")
                        {
                            identityUser.Email = value;
                            identityUser.NormalizedEmail = value.ToUpper();
                        }
                        
                        if (property.Name == "Username")
                        {
                            identityUser.UserName = value;
                            identityUser.NormalizedUserName = value.ToUpper();
                        }
                 }
     
                 // var identityUserProperty = typeof(IdentityUser).GetProperty(property.Name);
                 // if (identityUserProperty != null && !string.IsNullOrWhiteSpace(value))
                 // {
                 //     identityUserProperty.SetValue(identityUser, value);
                 // }
             }
         }
     }