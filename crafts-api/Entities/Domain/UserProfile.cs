using crafts_api.models.domain;

namespace crafts_api.Entities.Domain
{
    public class UserProfile
    {
        // id
        public int Id { get; set; }
        // public id
        public Guid UserPublicId { get; set; }
        public virtual User User { get; set; }
        // profile_picture
        public string ProfilePicture { get; set; }
        // country
        public string Country { get; set; }
        // city
        public string City { get; set; }
        // address
        public string Address { get; set; }
        // street
        public string Street { get; set; }
        // number
        public string Number { get; set; }
        // postal_code
        public string PostalCode { get; set; }
        // phone_number
        public string PhoneNumber { get; set; }
    }
}
