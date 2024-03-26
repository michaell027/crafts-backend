using crafts_api.context;
using crafts_api.interfaces;
using crafts_api.models.dto;
using Microsoft.EntityFrameworkCore;

namespace crafts_api.services
{
    public class CategoryService : ICategoryService
    {
        private readonly DatabaseContext _databaseContext;

        public CategoryService(DatabaseContext databaseContext) => _databaseContext = databaseContext;

        
        // create category
        public async Task<CategoryDto> CreateCategoryAsync(models.models.CategoryModel categoryModel)
        {
            var category = new models.domain.Category
            {
                Name = categoryModel.Name,
                SkName = categoryModel.SkName,
            };
            
            await _databaseContext.Categories.AddAsync(category);
            await _databaseContext.SaveChangesAsync();
            return category.ToDto();
        }

        // get all categories
        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            return await _databaseContext.Categories
                .AsNoTracking()
                .OrderBy(category => category.Id)
                .Select(category => category.ToDto())
                .ToListAsync();
        }
    }
}