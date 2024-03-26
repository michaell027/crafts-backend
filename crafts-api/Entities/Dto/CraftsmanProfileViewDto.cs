using crafts_api.Entities.Domain;

namespace crafts_api.Entities.Dto
{
    public class CraftsmanProfileViewDto
    {
        public Guid PublicId { get; set; }
        public string Bio { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ProfilePicture { get; set; }
        public List<CraftsmanServiceDto> CraftsmanServices { get; set; } = new List<CraftsmanServiceDto>();
    }
}
