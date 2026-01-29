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

    [HttpPost]
    public async Task<IActionResult> CreateImage([FromBody] CreateImageRequest request)
    {
        var result = await _imageService.CreateImageAsync(request);
        return Ok(result);
    }

    [HttpGet("product/{productId}")]
    public async Task<IActionResult> GetByProduct(Guid productId)
    {
        var result = await _imageService.GetImagesByProductAsync(productId);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _imageService.DeleteImageAsync(id);
        return NoContent();
    }
}