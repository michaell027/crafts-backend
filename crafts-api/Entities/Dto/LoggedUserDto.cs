using crafts_api.Entities.Enum;

namespace crafts_api.Entities.Dto
{
    public class LoggedUserDto
    {
        public Guid PublicId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public Role Role { get; set; }
    }
}