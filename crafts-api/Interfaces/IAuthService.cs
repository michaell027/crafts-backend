using crafts_api.Entities.Models;

namespace crafts_api.interfaces
{
    public interface IAuthService
    {
        Task CraftsmanRegister(RegisterCraftsmanRequest registerCraftsmanRequest);
        Task UserRegister(RegisterUserRequest registerUserRequest);        
        Task<LoggedUser> Login(LoginRequest loginRequest);
        Task<LoggedUser> RefreshToken(RefreshTokenRequest refreshTokenRequest);
        Task Revoke(string token);

    }
}
