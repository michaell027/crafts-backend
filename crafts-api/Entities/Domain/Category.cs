using crafts_api.models.dto;
using System.ComponentModel.DataAnnotations;

namespace crafts_api.models.domain;

public class Category
{
    // id
    [Key]
    public int Id { get; set; }

    // public id
    public Guid PublicId { get; set; }

    // name
    public string Name { get; set; }

    // sk_name
    public string SkName { get; set; }

    // to dto
    public CategoryDto ToDto() => new()
    {
        PublicId = PublicId,
        Name = Name,
        SkName = SkName
    };

}