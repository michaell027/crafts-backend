using crafts_api.models.domain;

namespace crafts_api.Entities.Domain
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = null!;
        public DateTime Expires { get; set; }
        public string UserIdentityId { get; set; }

        public virtual User User { get; set; }
    }
}

