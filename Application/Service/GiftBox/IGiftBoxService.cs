using Application.DTOs.Request.GiftBox;
using Application.DTOs.Response.GiftBox;

namespace Application.Service.GiftBox
{
    public interface IGiftBoxService
    {
        Task<IEnumerable<GiftBoxResponse>> GetAllGiftBoxesAsync();
        Task<GiftBoxResponse?> GetGiftBoxByIdAsync(Guid id);
        Task<GiftBoxResponse?> GetGiftBoxByCodeAsync(string code);
        Task<IEnumerable<GiftBoxResponse>> GetGiftBoxesByCategoryAsync(Guid categoryId);
        Task<IEnumerable<GiftBoxResponse>> GetActiveGiftBoxesAsync();
        Task<IEnumerable<GiftBoxResponse>> GetGiftBoxesByUserIdAsync(Guid userId);
        Task<GiftBoxResponse> CreateGiftBoxAsync(CreateGiftBoxRequest request);
        Task<GiftBoxResponse?> UpdateGiftBoxAsync(Guid id, UpdateGiftBoxRequest request);
        Task<bool> DeleteGiftBoxAsync(Guid id);
    }
}
