using crafts_api.Entities.Dto;
using crafts_api.Entities.Models;

namespace crafts_api.Interfaces;

public interface IUserService
{
    public Task<UserDto> GetUser(Guid publicId);
}