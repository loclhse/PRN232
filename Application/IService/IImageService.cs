using Application.DTOs.Request;
using Application.DTOs.Response;

namespace Application.IService
{
    public interface IImageService
    {
        Task<ImageResponse> CreateImageAsync(CreateImageRequest request);
        Task<IEnumerable<ImageResponse>> GetImagesByProductAsync(Guid productId);
        Task DeleteImageAsync(Guid id);
    }
}