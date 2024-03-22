using crafts_api.models.domain;
using crafts_api.models.models;

namespace crafts_api.interfaces
{
    public interface IAuthService
    {
        Task Register(RegisterRequest registerRequest);
        Task<User> Login(LoginRequest loginRequest);
        void TestError();
    }
}
