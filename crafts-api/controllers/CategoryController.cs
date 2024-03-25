using crafts_api.interfaces;
using crafts_api.models.dto;
using crafts_api.models.models;
using Microsoft.AspNetCore.Mvc;

namespace crafts_api.controllers;

/// <summary>
/// Category controller
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class CategoryController: ControllerBase
{
    private readonly ICategoryService _categoryService;

    /// <summary>
    /// CategoryController constructor
    /// </summary>
    /// <param name="categoryService"></param>
    public CategoryController(ICategoryService categoryService) =>
        _categoryService = categoryService;

    /// <summary>
    /// Get all categories
    /// </summary>
    /// <returns></returns>
    [HttpGet ("categories")]
    public IActionResult GetCategories()
    {
        List<CategoryDto> categoryDtos = _categoryService.GetAllCategoriesAsync().Result.ToList();
        
        if (categoryDtos.Count == 0)
        {
            return NotFound();
        }

        return Ok(categoryDtos);
    }
    
    /// <summary>
    /// Create a new category
    /// </summary>
    /// <param name="categoryModel"></param>
    /// <returns></returns>
    [HttpPost ("create-category")]
    public IActionResult CreateCategory(CategoryModel categoryModel)
    {
        CategoryDto categoryDto = _categoryService.CreateCategoryAsync(categoryModel).Result;

        if (categoryDto == null)
        {
            return BadRequest();
        }

        return Ok(categoryDto);
    }
    
}