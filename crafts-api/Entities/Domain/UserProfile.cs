using crafts_api.models.domain;

namespace crafts_api.Entities.Domain
{
    public class UserProfile
    {
        public int Id { get; set; }
        public Guid UserPublicId { get; set; }
        public User User { get; set; }
        public string ProfilePicture { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
        public string PostalCode { get; set; }
        public string PhoneNumber { get; set; }
    }
}
