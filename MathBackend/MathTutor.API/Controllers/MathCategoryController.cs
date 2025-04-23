using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MathTutor.Application.Interfaces;
using MathTutor.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathTutor.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MathCategoryController : ControllerBase
    {
        private readonly IMathCategoryService _categoryService;
        private readonly ILogger<MathCategoryController> _logger;

        public MathCategoryController(IMathCategoryService categoryService, ILogger<MathCategoryController> logger)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MathCategoryModel>>> GetAllCategories()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all math categories");
                return StatusCode(500, "An error occurred while retrieving math categories");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MathCategoryModel>> GetCategoryById(int id)
        {
            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id);
                if (category == null)
                {
                    return NotFound($"Math category with ID {id} not found");
                }
                return Ok(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving math category with ID {CategoryId}", id);
                return StatusCode(500, "An error occurred while retrieving the math category");
            }
        }

        [HttpPost]
        public async Task<ActionResult<MathCategoryModel>> CreateCategory([FromBody] MathCategoryModel categoryModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdCategory = await _categoryService.CreateCategoryAsync(categoryModel);
                return CreatedAtAction(nameof(GetCategoryById), new { id = createdCategory.Id }, createdCategory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating math category");
                return StatusCode(500, "An error occurred while creating the math category");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] MathCategoryModel categoryModel)
        {
            try
            {
                if (id != categoryModel.Id)
                {
                    return BadRequest("ID mismatch between route and model");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updated = await _categoryService.UpdateCategoryAsync(id, categoryModel);
                if (!updated)
                {
                    return NotFound($"Math category with ID {id} not found");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating math category with ID {CategoryId}", id);
                return StatusCode(500, "An error occurred while updating the math category");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var deleted = await _categoryService.DeleteCategoryAsync(id);
                if (!deleted)
                {
                    return NotFound($"Math category with ID {id} not found");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting math category with ID {CategoryId}", id);
                return StatusCode(500, "An error occurred while deleting the math category");
            }
        }
    }
} 