using crafts_api.models.dto;
using crafts_api.models.models;

namespace crafts_api.interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto> CreateCategoryAsync(CategoryModel categoryModel);
    }
}
