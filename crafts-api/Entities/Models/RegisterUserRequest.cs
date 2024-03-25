using crafts_api.Entities.Enum;

namespace crafts_api.models.models
{
    public class RegisterUserRequest
    {
        //user
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string PasswordConfirmation { get; set; }
        public string Email { get; set; }

        //user_profile
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
