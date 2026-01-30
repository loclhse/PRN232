using Application.DTOs.Request.Image;
using Application.DTOs.Response.Image;
using Application.DTOs.Response;
using Application.Service.Image;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class ImageController : ControllerBase
{
    private readonly IImageService _imageService;

    public ImageController(IImageService imageService)
    {
        _imageService = imageService;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ImageResponse>>> CreateImage([FromBody] CreateImageRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
            return BadRequest(ApiResponse<ImageResponse>.FailureResponse("Validation failed", errors));
        }

        try
        {
            var result = await _imageService.CreateImageAsync(request);
            return Ok(ApiResponse<ImageResponse>.SuccessResponse(result, "Image created successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<ImageResponse>.FailureResponse("An error occurred while creating image.", new List<string> { ex.Message }));
        }
    }

    [HttpGet("product/{productId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ImageResponse>>>> GetByProduct(Guid productId)
    {
        try
        {
            var result = await _imageService.GetImagesByProductAsync(productId);
            return Ok(ApiResponse<IEnumerable<ImageResponse>>.SuccessResponse(result, "Images retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<ImageResponse>>.FailureResponse("An error occurred while retrieving images.", new List<string> { ex.Message }));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> Delete(Guid id)
    {
        try
        {
            await _imageService.DeleteImageAsync(id);
            return Ok(ApiResponse.SuccessResponse("Image deleted successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse.FailureResponse(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.FailureResponse("An error occurred while deleting image.", new List<string> { ex.Message }));
        }
    }

}