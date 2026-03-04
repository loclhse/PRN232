using Application.DTOs.Request.GiftBoxComponentConfig;
using Application.DTOs.Response.GiftBoxComponentConfig;

namespace Application.Service.GiftBoxComponentConfig
{
    public interface IGiftBoxComponentConfigService
    {
        Task<IEnumerable<GiftBoxComponentConfigResponse>> GetAllAsync();
        Task<GiftBoxComponentConfigResponse?> GetByIdAsync(Guid id);
        Task<IEnumerable<GiftBoxComponentConfigResponse>> GetByCategoryAsync(string category);
        Task<IEnumerable<GiftBoxComponentConfigResponse>> GetActiveConfigsAsync();
        Task<GiftBoxComponentConfigResponse> CreateAsync(CreateGiftBoxComponentConfigRequest request);
        Task<GiftBoxComponentConfigResponse?> UpdateAsync(Guid id, UpdateGiftBoxComponentConfigRequest request);
        Task<bool> DeleteAsync(Guid id);
    }
}
