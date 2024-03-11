using crafts_api.configuration;
using crafts_api.context;
using crafts_api.models.domain;
using MySqlConnector;
using System.Collections.Generic;
using crafts_api.models.models;

namespace crafts_api.services
{
    public class CategoryService
    {
        private readonly DatabaseContext _databaseContext;

        public CategoryService(DatabaseContext databaseContext) => _databaseContext = databaseContext;

        // get all categories
        public List<Category> GetAllCategories()
        {
            return _databaseContext.Categories.ToList();
        }
        
        // create category
        public Category CreateCategory(CategoryModel categoryModel)
        {
            var category = new Category
            {
                PublicId = Guid.NewGuid(),
                CategoryName = categoryModel.CategoryName,
                SkName = categoryModel.SkName,
                Description = categoryModel.Description
            };
            _databaseContext.Categories.Add(category);
            _databaseContext.SaveChanges();
            return category;
        }
    }
}