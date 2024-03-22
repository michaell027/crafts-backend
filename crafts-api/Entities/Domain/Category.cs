using crafts_api.models.dto;

namespace crafts_api.models.domain;

public class Category
{
    // id
    public int Id { get; set; }

    // public id
    public Guid PublicId { get; set; } = Guid.NewGuid();

    // name
    public string Name { get; set; } = string.Empty;

    // sk_name
    public string SkName { get; set; } = string.Empty;

    // to dto
    public CategoryDto ToDto() => new()
    {
        PublicId = PublicId,
        Name = Name,
        SkName = SkName
    };

}