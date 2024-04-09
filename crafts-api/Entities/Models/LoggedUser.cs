using crafts_api.models.dto;

namespace crafts_api.Entities.Models
{
    public class LoggedUser
    {
        public LoggedUserDto User { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
