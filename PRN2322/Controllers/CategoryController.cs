using Application.DTOs.Request;
using Application.DTOs.Request.Category;
using Application.DTOs.Response;
using Application.Service.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PRN2322.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Lấy danh sách tất cả danh mục
        /// </summary>
        [HttpGet("GetAllCategories")]
        public async Task<ActionResult<ApiResponse<IEnumerable<CategoryResponse>>>> GetAllCategories()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                return Ok(ApiResponse<IEnumerable<CategoryResponse>>.SuccessResponse(categories, "Categories retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<CategoryResponse>>.FailureResponse("An error occurred while retrieving categories.", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Lấy thông tin danh mục theo ID
        /// </summary>
        [HttpGet("GetCategoryById/{id}")]
        public async Task<ActionResult<ApiResponse<CategoryResponse>>> GetCategoryById(Guid id)
        {
            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id);

                if (category == null)
                {
                    return NotFound(ApiResponse<CategoryResponse>.FailureResponse("Category not found."));
                }

                return Ok(ApiResponse<CategoryResponse>.SuccessResponse(category, "Category retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<CategoryResponse>.FailureResponse("An error occurred while retrieving category.", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Tạo danh mục mới
        /// </summary>
        [HttpPost("CreateCategory")]
        //[Authorize]
        public async Task<ActionResult<ApiResponse<CategoryResponse>>> CreateCategory([FromBody] CreateCategoryRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                return BadRequest(ApiResponse<CategoryResponse>.FailureResponse("Validation failed", errors));
            }

            try
            {
                var category = await _categoryService.CreateCategoryAsync(request);
                return CreatedAtAction(
                    nameof(GetCategoryById), 
                    new { id = category.Id }, 
                    ApiResponse<CategoryResponse>.SuccessResponse(category, "Category created successfully")
                );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<CategoryResponse>.FailureResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<CategoryResponse>.FailureResponse("An error occurred while creating the category.", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Cập nhật thông tin danh mục
        /// </summary>
        [HttpPut("UpdateCategory/{id}")]
        //[Authorize]
        public async Task<ActionResult<ApiResponse<CategoryResponse>>> UpdateCategory(Guid id, [FromBody] UpdateCategoryRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                return BadRequest(ApiResponse<CategoryResponse>.FailureResponse("Validation failed", errors));
            }

            try
            {
                var category = await _categoryService.UpdateCategoryAsync(id, request);

                if (category == null)
                {
                    return NotFound(ApiResponse<CategoryResponse>.FailureResponse("Category not found."));
                }

                return Ok(ApiResponse<CategoryResponse>.SuccessResponse(category, "Category updated successfully"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<CategoryResponse>.FailureResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<CategoryResponse>.FailureResponse("An error occurred while updating the category.", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Xóa danh mục (soft delete)
        /// </summary>
        [HttpDelete("DeleteCategory/{id}")]
        //[Authorize]
        public async Task<ActionResult<ApiResponse>> DeleteCategory(Guid id)
        {
            try
            {
                var result = await _categoryService.DeleteCategoryAsync(id);

                if (!result)
                {
                    return NotFound(ApiResponse.FailureResponse("Category not found."));
                }

                return Ok(ApiResponse.SuccessResponse("Category deleted successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.FailureResponse("An error occurred while deleting category.", new List<string> { ex.Message }));
            }
        }
    }
}
