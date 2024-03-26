namespace crafts_api.Entities.Dto
{
    public class CraftsmanServiceDto
    {
        // public id
        public Guid ServicePublicId { get; set; }
        // name
        public string Name { get; set; }
        // description
        public string Description { get; set; }
        // category
        public Guid CategoryPublicId { get; set; }
        public string CategoryName { get; set; }
        // price
        public decimal Price { get; set; }
        // duration
        public int Duration { get; set; }

    }
}
