using Application.DTOs.Request.GiftBoxComponentConfig;
using Application.DTOs.Response.GiftBoxComponentConfig;
using AutoMapper;
using Domain.IUnitOfWork;

namespace Application.Service.GiftBoxComponentConfig
{
    using GiftBoxComponentConfigEntity = Domain.Entities.GiftBoxComponentConfig;

    public class GiftBoxComponentConfigService : IGiftBoxComponentConfigService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GiftBoxComponentConfigService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GiftBoxComponentConfigResponse>> GetAllAsync()
        {
            var configs = await _unitOfWork.GiftBoxComponentConfigRepository.FindAsync(
                filter: c => !c.IsDeleted,
                includeProperties: "GiftBox"
            );

            return _mapper.Map<IEnumerable<GiftBoxComponentConfigResponse>>(configs);
        }

        public async Task<GiftBoxComponentConfigResponse?> GetByIdAsync(Guid id)
        {
            var config = await _unitOfWork.GiftBoxComponentConfigRepository.GetByIdWithGiftBoxAsync(id);

            if (config == null)
                return null;

            return _mapper.Map<GiftBoxComponentConfigResponse>(config);
        }

        public async Task<IEnumerable<GiftBoxComponentConfigResponse>> GetByCategoryAsync(string category)
        {
            var configs = await _unitOfWork.GiftBoxComponentConfigRepository.GetByCategoryAsync(category);
            return _mapper.Map<IEnumerable<GiftBoxComponentConfigResponse>>(configs);
        }

        public async Task<IEnumerable<GiftBoxComponentConfigResponse>> GetActiveConfigsAsync()
        {
            var configs = await _unitOfWork.GiftBoxComponentConfigRepository.GetActiveConfigsAsync();
            return _mapper.Map<IEnumerable<GiftBoxComponentConfigResponse>>(configs);
        }

        public async Task<GiftBoxComponentConfigResponse> CreateAsync(CreateGiftBoxComponentConfigRequest request)
        {
            var config = _mapper.Map<GiftBoxComponentConfigEntity>(request);
            config.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.GiftBoxComponentConfigRepository.AddAsync(config);
            await _unitOfWork.SaveChangesAsync();

            // Reload with related entities
            var createdConfig = await _unitOfWork.GiftBoxComponentConfigRepository.GetByIdWithGiftBoxAsync(config.Id);

            return _mapper.Map<GiftBoxComponentConfigResponse>(createdConfig);
        }

        public async Task<GiftBoxComponentConfigResponse?> UpdateAsync(Guid id, UpdateGiftBoxComponentConfigRequest request)
        {
            var config = await _unitOfWork.GiftBoxComponentConfigRepository.GetFirstOrDefaultAsync(
                filter: c => c.Id == id && !c.IsDeleted
            );

            if (config == null)
                return null;

            _mapper.Map(request, config);
            config.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.GiftBoxComponentConfigRepository.Update(config);
            await _unitOfWork.SaveChangesAsync();

            // Reload with related entities
            var updatedConfig = await _unitOfWork.GiftBoxComponentConfigRepository.GetByIdWithGiftBoxAsync(id);

            return _mapper.Map<GiftBoxComponentConfigResponse>(updatedConfig);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var config = await _unitOfWork.GiftBoxComponentConfigRepository.GetFirstOrDefaultAsync(
                filter: c => c.Id == id && !c.IsDeleted,
                includeProperties: "GiftBox"
            );

            if (config == null)
                return false;

            // Check if config is linked to a GiftBox
            if (config.GiftBox != null)
            {
                throw new InvalidOperationException($"Cannot delete GiftBoxComponentConfig '{config.Name}' because it is linked to GiftBox '{config.GiftBox.Name}'.");
            }

            // Soft delete
            config.IsDeleted = true;
            config.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.GiftBoxComponentConfigRepository.Update(config);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
