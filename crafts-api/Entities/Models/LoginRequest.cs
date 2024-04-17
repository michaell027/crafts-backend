using crafts_api.Entities.Enum;

namespace crafts_api.Entities.Models;

public class LoginRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public Role Role { get; set; }
}