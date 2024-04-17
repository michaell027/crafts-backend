
using crafts_api.Entities.Dto;

namespace crafts_api.Entities.Models
{
    public class LoggedUser
    {
        public LoggedUserDto User { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
