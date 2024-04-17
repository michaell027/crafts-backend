using crafts_api.Entities.Dto;
using crafts_api.Entities.Models;

namespace crafts_api.interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto> CreateCategoryAsync(CategoryModel categoryModel);
    }
}
