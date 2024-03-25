namespace crafts_api.Entities.Domain
{
    public class CraftsmanService
    {
        // id
        public int Id { get; set; }
        // public id
        public Guid CraftsmanProfileCraftsmanPublicId { get; set; }
        public CraftsmanProfile CraftsmanProfile { get; set; }
        // service_public_id
        public Guid ServicePublicId { get; set; }
        // service
        public Service Service { get; set; }
        // price
        public decimal Price { get; set; }
        // duration
        public int Duration { get; set; }
        // craftsman_service_availability
        // public List<CraftsmanServiceAvailability> CraftsmanServiceAvailability { get; set; }
    }
}
