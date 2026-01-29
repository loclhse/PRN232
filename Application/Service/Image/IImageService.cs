using Application.DTOs.Request.Image;
using Application.DTOs.Response.Image;

namespace Application.Service.Image
{
    public interface IImageService
    {
        Task<ImageResponse> CreateImageAsync(CreateImageRequest request);
        Task<IEnumerable<ImageResponse>> GetImagesByProductAsync(Guid productId);
        Task<IEnumerable<ImageResponse>> GetAllImagesAsync();
        Task UpdateImageAsync(Guid id, UpdateImageRequest request);
        Task DeleteImageAsync(Guid id);
    }
}