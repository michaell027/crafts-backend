using crafts_api.Entities.Domain;
using crafts_api.Entities.Dto;
using crafts_api.Entities.Enum;
using crafts_api.Interfaces;

namespace crafts_api.models.domain
{
    public class User
    {
        //id
        public int Id { get; set; }
        //public id
        public Guid PublicId { get; set; }
        //identity_id
        public string IdentityId { get; set; } = null!;
        //username
        public string Username { get; set; } = null!;
        //first_name
        public string FirstName { get; set; } = null!;
        //last_name
        public string LastName { get; set; } = null!;
        //email
        public string Email { get; set; } = null!;
        //created_at
        public DateTime CreatedAt { get; set; }
        //updated_at
        public DateTime UpdatedAt { get; set;}
        //role
        public Role Role { get; set; }
        //user_profile
        public virtual UserProfile UserProfile { get; set; } = new UserProfile();
        
        public UserDto ToDto() => new()
        {
            PublicId = PublicId,
            Username = Username,
            FirstName = FirstName,
            LastName = LastName,
            Email = Email,
            CreatedAt = CreatedAt,
            UpdatedAt = UpdatedAt,
            Role = Role,
            ProfilePicture = UserProfile.ProfilePicture,
            Country = UserProfile.Country,
            City = UserProfile.City,
            Address = UserProfile.Address,
            Street = UserProfile.Street,
            Number = UserProfile.Number,
            PostalCode = UserProfile.PostalCode,
            PhoneNumber = UserProfile.PhoneNumber
        };
    }
}
