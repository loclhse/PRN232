using Application.DTOs.Request.Image;
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

    [HttpGet("GetAllImage")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _imageService.GetAllImagesAsync();
        return Ok(result);
    }


    [HttpGet("GetImageByProductId/{productId}")]
    public async Task<IActionResult> GetByProduct(Guid productId)
    {
        var result = await _imageService.GetImagesByProductAsync(productId);
        return Ok(result);
    }

    [HttpPost("CreateImage")]
    public async Task<IActionResult> CreateImage([FromBody] CreateImageRequest request)
    {
        var result = await _imageService.CreateImageAsync(request);
        return Ok(result);
    }

    [HttpPut("UpdateImage/{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateImageRequest request)
    {
        try
        {
            await _imageService.UpdateImageAsync(id, request);
            return Ok(new { Message = "Update image successfully" });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { Message = "Image not found" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpDelete("DeleteImage{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _imageService.DeleteImageAsync(id);
        return NoContent();
    }

}