using crafts_api.models.domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace crafts_api.Entities.Domain
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = null!;
        public DateTime Expires { get; set; }
        public string UserIdentityId { get; set; }

        public User User { get; set; }
    }
}

