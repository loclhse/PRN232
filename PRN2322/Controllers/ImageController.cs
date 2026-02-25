using Application.DTOs.Request.Image;
using Application.DTOs.Response.Image;
using Application.DTOs.Response;
using Application.Service.Image;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    // RESTful API standard: use plural nouns for route names
    [Route("api/images")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }

        // GET: api/images
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<ImageResponse>>>> GetImages()
        {
            try
            {
                var result = await _imageService.GetAllImagesAsync();
                return Ok(ApiResponse<IEnumerable<ImageResponse>>.SuccessResponse(result, "Tải danh sách ảnh thành công."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<ImageResponse>>.FailureResponse("Lỗi hệ thống khi lấy danh sách ảnh.", new List<string> { ex.Message }));
            }
        }

        // POST: api/images
        [HttpPost]
        public async Task<ActionResult<ApiResponse<ImageResponse>>> CreateImage([FromBody] CreateImageRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                return BadRequest(ApiResponse<ImageResponse>.FailureResponse("Dữ liệu đầu vào không hợp lệ.", errors));
            }

            try
            {
                var result = await _imageService.CreateImageAsync(request);
                // Lưu ý: Nếu có hàm GetById, nên dùng CreatedAtAction ở đây để trả về status 201
                return Ok(ApiResponse<ImageResponse>.SuccessResponse(result, "Tạo ảnh mới thành công."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<ImageResponse>.FailureResponse("Lỗi hệ thống khi tạo ảnh.", new List<string> { ex.Message }));
            }
        }

        // PUT: api/images/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> UpdateImage(Guid id, [FromBody] UpdateImageRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                return BadRequest(ApiResponse<string>.FailureResponse("Dữ liệu cập nhật không hợp lệ.", errors));
            }

            try
            {
                await _imageService.UpdateImageAsync(id, request);
                return Ok(ApiResponse<string>.SuccessResponse("Cập nhật thông tin ảnh thành công."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<string>.FailureResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailureResponse("Lỗi hệ thống khi cập nhật ảnh.", new List<string> { ex.Message }));
            }
        }

        // GET: api/images/products/{productId}
        // Đổi "product" thành "products" để đồng bộ số nhiều
        [HttpGet("products/{productId}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ImageResponse>>>> GetImagesByProduct(Guid productId)
        {
            try
            {
                var result = await _imageService.GetImagesByProductAsync(productId);
                return Ok(ApiResponse<IEnumerable<ImageResponse>>.SuccessResponse(result, "Lấy danh sách ảnh theo sản phẩm thành công."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<ImageResponse>>.FailureResponse("Lỗi hệ thống khi lấy ảnh theo sản phẩm.", new List<string> { ex.Message }));
            }
        }

        // DELETE: api/images/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteImage(Guid id)
        {
            try
            {
                await _imageService.DeleteImageAsync(id);
                return Ok(ApiResponse.SuccessResponse("Xóa ảnh thành công."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse.FailureResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.FailureResponse("Lỗi hệ thống khi xóa ảnh.", new List<string> { ex.Message }));
            }
        }
    }
}