namespace crafts_api.models;

public class LoginRequest
{
    public required string Credential { get; set; }
    public required string Password { get; set; }
}