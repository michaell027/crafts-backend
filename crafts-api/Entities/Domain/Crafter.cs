using crafts_api.models.domain;

namespace crafts_api.Entities.Domain
{
    public class Crafter
    {
        // id
        public int Id { get; set; }
        // public id
        public Guid PublicId { get; set; }
        // user
        public Guid UserPublicId { get; set; }
        public User User { get; set; } = null!;
        // category
        public Guid CategoryPublicId { get; set; }
        public Category Category { get; set; } = null!;
    }
}
