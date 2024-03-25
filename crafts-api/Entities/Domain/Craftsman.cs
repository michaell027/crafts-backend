using crafts_api.Entities.Enum;
using crafts_api.models.domain;

namespace crafts_api.Entities.Domain
{
    public class Craftsman
    {
        // id
        public int Id { get; set; }
        // public id
        public Guid PublicId { get; set; }
        // identity_id
        public string IdentityId { get; set; }
        // first_name
        public string FirstName { get; set; }
        // last_name
        public string LastName { get; set; }
        // email
        public string Email { get; set; }
        // created_at
        public DateTime CreatedAt { get; set; }
        // updated_at
        public DateTime UpdatedAt { get; set; }
        // role
        public Role Role { get; set; }
        // username
        public string Username { get; set; }
        // craftsman_profile
        public CraftsmanProfile CraftsmanProfile { get; set; }
        // craftsman_reviews
        // public List<CraftsmanReview> CraftsmanReviews { get; set; }
        // craftsman_ratings
        // public List<CraftsmanRating> CraftsmanRatings { get; set; }
        // craftman_gallery
        // public List<CraftsmanGallery> CraftsmanGallery { get; set; }
        // craftsman_certificates
        // public List<CraftsmanCertificate> CraftsmanCertificates { get; set; }
        // craftsman_works
        // public List<CraftsmanWork> CraftsmanWorks { get; set; }
        // craftsman_availability
        // public List<CraftsmanAvailability> CraftsmanAvailability { get; set; }
    }
}
