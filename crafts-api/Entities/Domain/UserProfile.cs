using crafts_api.models.domain;

namespace crafts_api.Entities.Domain
{
    public class UserProfile
    {
        public int Id { get; set; }
        public Guid PublicId { get; set; }
        public User user { get; set; }
        public string ProfilePicture { get; set; }
        public string Country { get; set; }
    }
}
