using System.Net;
using crafts_api.context;
using crafts_api.Entities.Dto;
using crafts_api.Entities.Enum;
using crafts_api.Entities.Models;
using crafts_api.exceptions;
using crafts_api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace crafts_api.services;

public class UserService : IUserService
{
    private readonly DatabaseContext _databaseContext;
    
    public UserService(DatabaseContext databaseContext) => _databaseContext = databaseContext;

    public async Task<UserDto> GetUser(Guid publicId)
    {
        if (publicId == Guid.Empty)
        {
            throw new DefaultException
            {
                StatusCode = HttpStatusCode.BadRequest,
                ErrorCode = 400,
                Message = "User public id is required"
            };
        }

        var user = await _databaseContext.Users
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
}