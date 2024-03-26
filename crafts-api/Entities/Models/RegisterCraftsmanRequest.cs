namespace crafts_api.Entities.Models
{
    public class RegisterCraftsmanRequest
    {
       // craftsman
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string PasswordConfirmation { get; set; }

        // craftsman_profile
        public string Bio { get; set; } = string.Empty;
        public required string PhoneNumber { get; set; }
        public string Address { get; set; } = string.Empty;
        public required string City { get; set; }
        public required string Country { get; set; }
        public required string Street { get; set; }
        public required string Number { get; set; }
        public required string PostalCode { get; set; }
        public string ProfilePicture { get; set; } = string.Empty;
    }
}
