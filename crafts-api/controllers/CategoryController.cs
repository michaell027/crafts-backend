using crafts_api.models.domain;
using crafts_api.models.models;
using crafts_api.services;
using Microsoft.AspNetCore.Mvc;

namespace crafts_api.controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController: ControllerBase
{
    private readonly CategoryService _categoryService;

    public CategoryController(CategoryService categoryService) =>
        _categoryService = categoryService;

    // GET CATEGORIES
    [HttpGet]
    public List<Category> GetCategories()
    {
        var categories = new List<Category>()
        {
            new Category { Id = 1, CategoryName = "Woodworking" },
            new Category { Id = 2, CategoryName = "Metalworking" },
            new Category { Id = 3, CategoryName = "Leatherworking" },
        };
        // return categories;
        return _categoryService.GetAllCategories();
    }
    
    // CREATE CATEGORY
    [HttpPost]
    public Category CreateCategory(CategoryModel categoryModel)
    {
        return _categoryService.CreateCategory(categoryModel);
    }
    
}