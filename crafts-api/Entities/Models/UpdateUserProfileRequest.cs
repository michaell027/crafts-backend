namespace crafts_api.Entities.Models
{
    public class UpdateUserProfileRequest
    {
        //username
        public string Username { get; set; } = string.Empty;
        //first_name
        public string FirstName { get; set; } = string.Empty;
        //last_name
        public string LastName { get; set; } = string.Empty;
        //email
        public string Email { get; set; } = string.Empty;
        //profile_picture
        public string ProfilePicture { get; set; } = string.Empty;
        //country
        public string Country { get; set; } = string.Empty;
        //city
        public string City { get; set; } = string.Empty;
        //address
        public string Address { get; set; } = string.Empty;
        //street
        public string Street { get; set; } = string.Empty;
        //number
        public string Number { get; set; } = string.Empty;
        //postal_code
        public string PostalCode { get; set; } = string.Empty;
        //phone_number
        public string PhoneNumber { get; set; } = string.Empty;

    }
}
