namespace crafts_api.models.domain;

public class Category
{
    // id
    public int Id { get; set; }
    
    // public id
    public Guid PublicId { get; set; }
    
    // name
    public string Name { get; set; }
    
    // sk_name
    public string SkName { get; set; }
    
    // description
    public string Description { get; set; }
}