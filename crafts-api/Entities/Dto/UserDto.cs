using crafts_api.Entities.Enum;

namespace crafts_api.Entities.Dto;

public class UserDto
{
    public Guid PublicId { get; set; }
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Role Role { get; set; }
    public string ProfilePicture { get; set; }
    public string Country { get; set; }
    public string City { get; set; }
    public string? Address { get; set; }
    public string Street { get; set; }
    public string Number { get; set; }
    public string PostalCode { get; set; }
    public string PhoneNumber { get; set; }
}