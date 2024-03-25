namespace crafts_api.Entities.Domain
{
    public class CraftsmanProfile
    {
        // id
        public int Id { get; set; }
        // public id
        public Guid CraftsmanPublicId { get; set; }
        public Craftsman Craftsman { get; set; }
        // description
        public string Bio { get; set; }
        // phone_number
        public string PhoneNumber { get; set; }
        // address
        public string Address { get; set; }
        // city
        public string City { get; set; }
        // country
        public string Country { get; set; }
        // street
        public string Street { get; set; }
        // number
        public string Number { get; set; }
        // postal_code
        public string PostalCode { get; set; }
        // profile_picture
        public string ProfilePicture { get; set; }
        // craftsman_services
        public List<CraftsmanService> CraftsmanServices { get; set; }
    }
}
