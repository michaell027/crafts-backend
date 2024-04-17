namespace crafts_api.Entities.Domain
{
    public class Service
    {
        // id
        public int Id { get; set; }
        // public id
        public Guid PublicId { get; set; }
        // name
        public string Name { get; set; }
        // description
        public string Description { get; set; }
        // category
        public Guid CategoryPublicId { get; set; }
        public virtual Category Category { get; set; }
    }
}
