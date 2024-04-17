using System.ComponentModel.DataAnnotations;
using crafts_api.Entities.Dto;

namespace crafts_api.Entities.Domain;

public class Category
{
    // id
    [Key]
    public int Id { get; set; }

    // public id
    public Guid PublicId { get; set; }

    // name
    public required string Name { get; set; }

    // sk_name
    public required string SkName { get; set; }

    // to dto
    public CategoryDto ToDto() => new()
    {
        PublicId = PublicId,
        Name = Name,
        SkName = SkName
    };

}