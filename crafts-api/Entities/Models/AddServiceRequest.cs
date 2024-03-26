namespace crafts_api.Entities.Models
{
    public class AddServiceRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid CategoryPublicId { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }
    }
}
