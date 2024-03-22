using crafts_api.models.dto;

namespace crafts_api.Entities.Models
{
    public class LoggedUser
    {
        public UserDto User { get; set; }
        public string Token { get; set; }
    }
}
