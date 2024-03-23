using crafts_api.Entities.Models;
using crafts_api.models.domain;
using crafts_api.models.models;

namespace crafts_api.interfaces
{
    public interface IAuthService
    {
        Task Register(RegisterRequest registerRequest);
        Task<LoggedUser> Login(LoginRequest loginRequest);
        Task<string> RefreshToken(RefreshTokenRequest refreshTokenRequest);
        void TestError();
    }
}
