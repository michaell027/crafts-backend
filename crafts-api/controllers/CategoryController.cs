using crafts_api.models;
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
            new Category { Id = 1, Name = "Woodworking" },
            new Category { Id = 2, Name = "Metalworking" },
            new Category { Id = 3, Name = "Leatherworking" },
            new Category { Id = 4, Name = "Pottery" },
            new Category { Id = 5, Name = "Glassworking" },
            new Category { Id = 6, Name = "Textile" },
            new Category { Id = 7, Name = "Jewelry" },
            new Category { Id = 8, Name = "Paper" },
            new Category { Id = 9, Name = "Candle" },
            new Category { Id = 10, Name = "Soap" }
        };
        // return categories;
        return _categoryService.GetAllCategories();
    }
    
}