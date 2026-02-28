using Application.DTOs.Request.Product;
using Application.DTOs.Response.Product;
using Application.DTOs.Response;
using Application.Service.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PRN2322.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Lấy danh sách tất cả sản phẩm
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProductResponse>>>> GetAllProducts()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                return Ok(ApiResponse<IEnumerable<ProductResponse>>.SuccessResponse(products, "Products retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<ProductResponse>>.FailureResponse("An error occurred while retrieving products.", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Lấy thông tin sản phẩm theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ProductResponse>>> GetProductById(Guid id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                
                if (product == null)
                {
                    return NotFound(ApiResponse<ProductResponse>.FailureResponse("Product not found."));
                }

                return Ok(ApiResponse<ProductResponse>.SuccessResponse(product, "Product retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<ProductResponse>.FailureResponse("An error occurred while retrieving product.", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Tạo sản phẩm mới
        /// </summary>
        [HttpPost]
        //[Authorize]
        public async Task<ActionResult<ApiResponse<ProductResponse>>> CreateProduct([FromBody] CreateProductRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                return BadRequest(ApiResponse<ProductResponse>.FailureResponse("Validation failed", errors));
            }

            try
            {
                var product = await _productService.CreateProductAsync(request);
                return CreatedAtAction(
                    nameof(GetProductById), 
                    new { id = product.Id }, 
                    ApiResponse<ProductResponse>.SuccessResponse(product, "Product created successfully")
                );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<ProductResponse>.FailureResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<ProductResponse>.FailureResponse("An error occurred while creating the product.", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Cập nhật thông tin sản phẩm
        /// </summary>
        [HttpPut]
        //[Authorize]
        public async Task<ActionResult<ApiResponse<ProductResponse>>> UpdateProduct(Guid id, [FromBody] UpdateProductRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                return BadRequest(ApiResponse<ProductResponse>.FailureResponse("Validation failed", errors));
            }

            try
            {
                var product = await _productService.UpdateProductAsync(id, request);
                
                if (product == null)
                {
                    return NotFound(ApiResponse<ProductResponse>.FailureResponse("Product not found."));
                }

                return Ok(ApiResponse<ProductResponse>.SuccessResponse(product, "Product updated successfully"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<ProductResponse>.FailureResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<ProductResponse>.FailureResponse("An error occurred while updating the product.", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Xóa sản phẩm (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        //[Authorize]
        public async Task<ActionResult<ApiResponse>> DeleteProduct(Guid id)
        {
            try
            {
                var result = await _productService.DeleteProductAsync(id);
                
                if (!result)
                {
                    return NotFound(ApiResponse.FailureResponse("Product not found."));
                }

                return Ok(ApiResponse.SuccessResponse("Product deleted successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.FailureResponse("An error occurred while deleting product.", new List<string> { ex.Message }));
            }
        }
    }
}
