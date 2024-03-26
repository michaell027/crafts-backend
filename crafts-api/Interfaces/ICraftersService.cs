using crafts_api.Entities.Dto;
using crafts_api.Entities.Models;

namespace crafts_api.Interfaces
{
    public interface ICraftersService
    {
        Task AddService(AddServiceRequest addServiceRequest, string token);
        Task<CraftsmanProfileViewDto> GetCraftsmanProfile(Guid craftsmanPublicId);
    }
}
