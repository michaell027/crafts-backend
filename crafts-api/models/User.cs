namespace crafts_api.models;

public class User
{
    //id
    public int Id { get; set; }
    //username
    public string Username { get; set; } = null!;
    //password
    public string Password { get; set; } = null!;
    //email
    public string? Email { get; set; }
    //created_at
    public DateTime CreatedAt { get; set; }
    //updated_at
    public DateTime UpdatedAt { get; set; }
}