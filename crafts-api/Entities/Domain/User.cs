namespace crafts_api.models.domain
{
    public class User
    {
        //id
        public int Id { get; set; }
        //public id
        public Guid PublicId { get; set; }
        //first_name
        public string FirstName { get; set; } = null!;
        //last_name
        public string LastName { get; set; } = null!;
        //password
        public string Password { get; set; } = null!;
        //email
        public string Email { get; set; } = null!;
        //created_at
        public DateTime CreatedAt { get; set; }
        //updated_at
        public DateTime UpdatedAt { get; set;}
    }
}
