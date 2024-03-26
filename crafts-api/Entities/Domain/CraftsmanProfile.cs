namespace crafts_api.Entities.Domain
{
    public class CraftsmanProfile
    {
        // id
        public int Id { get; set; }
        // public id
        public Guid CraftsmanPublicId { get; set; }
        public virtual Craftsman Craftsman { get; set; } = null!;
        // description
        public string Bio { get; set; } = string.Empty;
        // phone_number
        public required string PhoneNumber { get; set; }
        // address
        public string Address { get; set; } = string.Empty;
        // city
        public required string City { get; set; }
        // country
        public required string Country { get; set; }
        // street
        public required string Street { get; set; }
        // number
        public required string Number { get; set; }
        // postal_code
        public required string PostalCode { get; set; }
        // profile_picture
        public string ProfilePicture { get; set; } = string.Empty;
        // craftsman_services
        public virtual List<CraftsmanService> CraftsmanServices { get; set; } = new List<CraftsmanService>();
    }
}
