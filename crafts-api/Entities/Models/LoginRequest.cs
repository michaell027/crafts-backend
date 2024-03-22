namespace crafts_api.models.models;

public class LoginRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}