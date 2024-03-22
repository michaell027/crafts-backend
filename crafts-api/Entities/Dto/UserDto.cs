namespace crafts_api.models.dto;

public class UserDto
{
    public Guid PublicId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
}