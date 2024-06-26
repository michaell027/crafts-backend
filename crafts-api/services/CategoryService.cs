using crafts_api.context;
using crafts_api.Entities.Domain;
using crafts_api.Entities.Dto;
using crafts_api.Entities.Models;
using crafts_api.interfaces;
using Microsoft.EntityFrameworkCore;

namespace crafts_api.services
{
    public class CategoryService : ICategoryService
    {
        private readonly DatabaseContext _databaseContext;

        public CategoryService(DatabaseContext databaseContext) => _databaseContext = databaseContext;

        
        // create category
        public async Task<CategoryDto> CreateCategoryAsync(CategoryModel categoryModel)
        {
            var category = new Category
            {
                PublicId = Guid.NewGuid(),
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