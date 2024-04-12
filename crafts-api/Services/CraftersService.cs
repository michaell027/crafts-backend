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

namespace crafts_api.Services
{
    public class CraftersService(IConfiguration configuration, DatabaseContext databaseContext) : ICraftersService
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
            throw new NotImplementedException();
        }
    }
}
