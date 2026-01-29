using Application.DTOs.Request.Image;
using Application.DTOs.Response.Image;

namespace Application.Service.Image
{
    public interface IImageService
    {
        Task<ImageResponse> CreateImageAsync(CreateImageRequest request);
        Task<IEnumerable<ImageResponse>> GetImagesByProductAsync(Guid productId);
        Task DeleteImageAsync(Guid id);
    }
}