using crafts_api.context;
using crafts_api.Entities.Domain;
using crafts_api.Entities.Dto;
using crafts_api.Entities.Enum;
using crafts_api.Entities.Models;
using crafts_api.exceptions;
using crafts_api.Interfaces;
using crafts_api.utils;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Microsoft.AspNetCore.Identity;

namespace crafts_api.Services
{
    public class CraftersService(IConfiguration configuration, DatabaseContext databaseContext, UserManager<IdentityUser> userManager) : ICraftersService
    {
        private readonly TokenFunctions _tokenFunctions = new(configuration);

        public async Task AddService(AddServiceRequest addServiceRequest, string token)
        {
            var craftsmanPublicId = _tokenFunctions.GetClaim(token, "nameid");

            if (craftsmanPublicId == null)
            {
                throw new DefaultException
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorCode = 400,
                    Message = "Craftsman public id is required"
                };
            }

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

            var service = new Service
            {
                PublicId = Guid.NewGuid(),
                Name = addServiceRequest.Name,
                Description = addServiceRequest.Description,
                CategoryPublicId = addServiceRequest.CategoryPublicId
            };

            var craftsmanService = new CraftsmanService
            {
                CraftsmanProfileCraftsmanPublicId = Guid.Parse(craftsmanPublicId),
                ServicePublicId = service.PublicId,
                Price = addServiceRequest.Price,
                Duration = addServiceRequest.Duration
            };

            await databaseContext.Services.AddAsync(service);
            await databaseContext.CraftsmanServices.AddAsync(craftsmanService);
            await databaseContext.SaveChangesAsync();
        }

        public async Task<CraftsmanProfileViewDto> GetCraftsmanProfile(Guid craftsmanPublicId)
        {
            if (craftsmanPublicId == Guid.Empty)
            {
                throw new DefaultException
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorCode = 400,
                    Message = "Craftsman public id is required"
                };
            }

            var craftsman = await databaseContext.Crafters
                .Include(x => x.CraftsmanProfile)
                .ThenInclude(x => x.CraftsmanServices)
                .ThenInclude(cs => cs.Service)
                .ThenInclude(s => s.Category)
                .FirstOrDefaultAsync(x => x.PublicId == craftsmanPublicId);

            if (craftsman == null)
            {
                throw new DefaultException
                {
                    StatusCode = HttpStatusCode.NotFound,
                    ErrorCode = 404,
                    Message = "Craftsman not found"
                };
            }

            var craftsmanServiceDtos = craftsman.CraftsmanProfile.CraftsmanServices
                .Select(x => new CraftsmanServiceDto
                {
                    ServicePublicId = x.Service.PublicId,
                    Name = x.Service.Name,
                    Description = x.Service.Description,
                    CategoryPublicId = x.Service.CategoryPublicId,
                    CategoryName = x.Service.Category.Name,
                    Price = x.Price,
                    Duration = x.Duration
                })
                .ToList();

            var craftsmanProfileViewDto = new CraftsmanProfileViewDto
            {
                PublicId = craftsman.PublicId,
                Bio = craftsman.CraftsmanProfile.Bio,
                PhoneNumber = craftsman.CraftsmanProfile.PhoneNumber,
                ProfilePicture = craftsman.CraftsmanProfile.ProfilePicture,
                FirstName = craftsman.FirstName,
                LastName = craftsman.LastName,
                UserName = craftsman.Username,
                CreatedAt = craftsman.CreatedAt,
                CraftsmanServices = craftsmanServiceDtos,
            };

            return craftsmanProfileViewDto;

        }
        
        public async Task UpdateCraftsmanProfile(UpdateCraftsmanProfileRequest updateCraftsmanProfileRequest, string token)
        {
            var craftsmanPublicId = _tokenFunctions.GetClaim(token, "nameid") ?? throw new DefaultException
            {
                StatusCode = HttpStatusCode.BadRequest,
                ErrorCode = 400,
                Message = "Craftsman public id is required"
            };
            var role = _tokenFunctions.GetClaim(token, "role") ?? throw new DefaultException
            {
                StatusCode = HttpStatusCode.BadRequest,
                ErrorCode = 400,
                Message = "Unauthorized"
            };
            
            if (role != Role.Crafter.ToString())
            {
                throw new DefaultException
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorCode = 400,
                    Message = "Unauthorized"
                };
            }
            
            if (updateCraftsmanProfileRequest.IsEmpty())
            {
                throw new DefaultException
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorCode = 400,
                    Message = "No fields to update"
                };
            }
            
            var craftsman = await databaseContext.Crafters
                .Include(x => x.CraftsmanProfile)
                .FirstOrDefaultAsync(x => x.PublicId == Guid.Parse(craftsmanPublicId))
                ?? throw new DefaultException
                {
                    StatusCode = HttpStatusCode.NotFound,
                    ErrorCode = 404,
                    Message = "Craftsman not found"
                };
            
            var identityUser = await userManager.FindByIdAsync(craftsman.IdentityId) ?? throw new DefaultException
            {
                StatusCode = HttpStatusCode.NotFound,
                ErrorCode = 404,
                Message = "User not found"
            };
            
            if (updateCraftsmanProfileRequest.Email != craftsman.Email && EmailExists(updateCraftsmanProfileRequest.Email))
            {
                throw new DefaultException
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorCode = 400,
                    Message = "Email already exists"
                };
            }
            
            updateCraftsmanProfileRequest.UpdateCraftsmanFields(craftsman, identityUser);
            craftsman.UpdatedAt = DateTime.Now;
            await databaseContext.SaveChangesAsync();
        }
        
        private bool EmailExists(string email)
        {
            return databaseContext.Crafters.Any(craftsman => craftsman.Email == email) ||
                   userManager.Users.Any(user => user.Email == email);
        }
    }

    public static class UpdateCraftsmanProfileExtensions
    {
        public static bool IsEmpty(this UpdateCraftsmanProfileRequest updateCraftsmanProfileRequest)
        {
            return string.IsNullOrEmpty(updateCraftsmanProfileRequest.Username) &&
                   string.IsNullOrEmpty(updateCraftsmanProfileRequest.FirstName) &&
                   string.IsNullOrEmpty(updateCraftsmanProfileRequest.LastName) &&
                   string.IsNullOrEmpty(updateCraftsmanProfileRequest.Email) &&
                   string.IsNullOrEmpty(updateCraftsmanProfileRequest.Bio) &&
                   string.IsNullOrEmpty(updateCraftsmanProfileRequest.PhoneNumber) &&
                   string.IsNullOrEmpty(updateCraftsmanProfileRequest.Address) &&
                   string.IsNullOrEmpty(updateCraftsmanProfileRequest.City) &&
                   string.IsNullOrEmpty(updateCraftsmanProfileRequest.Country) &&
                   string.IsNullOrEmpty(updateCraftsmanProfileRequest.Street) &&
                   string.IsNullOrEmpty(updateCraftsmanProfileRequest.Number) &&
                   string.IsNullOrEmpty(updateCraftsmanProfileRequest.PostalCode) &&
                   string.IsNullOrEmpty(updateCraftsmanProfileRequest.ProfilePicture);
        }

        public static void UpdateCraftsmanFields(this UpdateCraftsmanProfileRequest updateCraftsmanProfileRequest,
            Craftsman craftsman, IdentityUser identityUser)
        {
            var properties = updateCraftsmanProfileRequest.GetType().GetProperties();

            foreach (var property in properties)
            {
                var value = property.GetValue(updateCraftsmanProfileRequest)?.ToString() ?? string.Empty;
                var craftsmanProperty = craftsman.GetType().GetProperty(property.Name);
                
                if (craftsmanProperty != null && !string.IsNullOrWhiteSpace(value))
                {
                    craftsmanProperty.SetValue(craftsman, value);

                    switch (property.Name)    
                    {
                        case "Email":
                            identityUser.Email = value;
                            identityUser.NormalizedEmail = value.ToUpper();
                            break;
                        case "Username":
                            identityUser.UserName = value;
                            identityUser.NormalizedUserName = value.ToUpper();
                            break;
                    }
                }
            }
        }
    }
}
