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
        return _categoryService.GetAllCategories();
    }
    
}