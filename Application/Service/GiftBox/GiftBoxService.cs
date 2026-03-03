using Application.DTOs.Request.GiftBox;
using Application.DTOs.Response.GiftBox;
using AutoMapper;
using Domain.Entities;
using Domain.IUnitOfWork;

namespace Application.Service.GiftBox
{
    using GiftBoxEntity = Domain.Entities.GiftBox;
    using CategoryEntity = Domain.Entities.Category;
    using GiftBoxComponentConfigEntity = Domain.Entities.GiftBoxComponentConfig;

    public class GiftBoxService : IGiftBoxService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GiftBoxService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GiftBoxResponse>> GetAllGiftBoxesAsync()
        {
            var giftBoxes = await _unitOfWork.GiftBoxRepository.FindAsync(
                filter: g => !g.IsDeleted,
                includeProperties: "Category,ComponentConfig,BoxComponents.Product,Images"
            );

            return _mapper.Map<IEnumerable<GiftBoxResponse>>(giftBoxes);
        }

        public async Task<GiftBoxResponse?> GetGiftBoxByIdAsync(Guid id)
        {
            var giftBox = await _unitOfWork.GiftBoxRepository.GetFirstOrDefaultAsync(
                filter: g => g.Id == id && !g.IsDeleted,
                includeProperties: "Category,ComponentConfig,BoxComponents.Product,Images"
            );

            if (giftBox == null)
                return null;

            return _mapper.Map<GiftBoxResponse>(giftBox);
        }

        public async Task<GiftBoxResponse?> GetGiftBoxByCodeAsync(string code)
        {
            var giftBox = await _unitOfWork.GiftBoxRepository.GetByCodeAsync(code);

            if (giftBox == null)
                return null;

            return _mapper.Map<GiftBoxResponse>(giftBox);
        }

        public async Task<IEnumerable<GiftBoxResponse>> GetGiftBoxesByCategoryAsync(Guid categoryId)
        {
            var giftBoxes = await _unitOfWork.GiftBoxRepository.GetByCategoryAsync(categoryId);
            return _mapper.Map<IEnumerable<GiftBoxResponse>>(giftBoxes);
        }

        public async Task<IEnumerable<GiftBoxResponse>> GetActiveGiftBoxesAsync()
        {
            var giftBoxes = await _unitOfWork.GiftBoxRepository.GetActiveGiftBoxesAsync();
            return _mapper.Map<IEnumerable<GiftBoxResponse>>(giftBoxes);
        }

        public async Task<GiftBoxResponse> CreateGiftBoxAsync(CreateGiftBoxRequest request)
        {
            // Check if Code already exists
            if (await _unitOfWork.GiftBoxRepository.CodeExistsAsync(request.Code))
            {
                throw new InvalidOperationException($"GiftBox with Code '{request.Code}' already exists.");
            }

            // Check if Category exists
            var category = await _unitOfWork.Repository<CategoryEntity>().GetByIdAsync(request.CategoryId);
            if (category == null || category.IsDeleted)
            {
                throw new InvalidOperationException($"Category with ID '{request.CategoryId}' not found.");
            }

            // Check if GiftBoxComponentConfig exists
            var componentConfig = await _unitOfWork.Repository<GiftBoxComponentConfigEntity>().GetByIdAsync(request.GiftBoxComponentConfigId);
            if (componentConfig == null || componentConfig.IsDeleted)
            {
                throw new InvalidOperationException($"GiftBoxComponentConfig with ID '{request.GiftBoxComponentConfigId}' not found.");
            }

            var giftBox = _mapper.Map<GiftBoxEntity>(request);
            giftBox.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.GiftBoxRepository.AddAsync(giftBox);
            await _unitOfWork.SaveChangesAsync();

            // Reload with related entities for response
            var createdGiftBox = await _unitOfWork.GiftBoxRepository.GetFirstOrDefaultAsync(
                filter: g => g.Id == giftBox.Id,
                includeProperties: "Category,ComponentConfig,BoxComponents.Product,Images"
            );

            return _mapper.Map<GiftBoxResponse>(createdGiftBox);
        }

        public async Task<GiftBoxResponse?> UpdateGiftBoxAsync(Guid id, UpdateGiftBoxRequest request)
        {
            var giftBox = await _unitOfWork.GiftBoxRepository.GetFirstOrDefaultAsync(
                filter: g => g.Id == id && !g.IsDeleted
            );

            if (giftBox == null)
                return null;

            // Check if Code already exists for another GiftBox
            if (await _unitOfWork.GiftBoxRepository.CodeExistsAsync(request.Code, id))
            {
                throw new InvalidOperationException($"GiftBox with Code '{request.Code}' already exists.");
            }

            // Check if Category exists
            var category = await _unitOfWork.Repository<CategoryEntity>().GetByIdAsync(request.CategoryId);
            if (category == null || category.IsDeleted)
            {
                throw new InvalidOperationException($"Category with ID '{request.CategoryId}' not found.");
            }

            // Check if GiftBoxComponentConfig exists
            var componentConfig = await _unitOfWork.Repository<GiftBoxComponentConfigEntity>().GetByIdAsync(request.GiftBoxComponentConfigId);
            if (componentConfig == null || componentConfig.IsDeleted)
            {
                throw new InvalidOperationException($"GiftBoxComponentConfig with ID '{request.GiftBoxComponentConfigId}' not found.");
            }

            _mapper.Map(request, giftBox);
            giftBox.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.GiftBoxRepository.Update(giftBox);
            await _unitOfWork.SaveChangesAsync();

            // Reload with related entities for response
            var updatedGiftBox = await _unitOfWork.GiftBoxRepository.GetFirstOrDefaultAsync(
                filter: g => g.Id == id,
                includeProperties: "Category,ComponentConfig,BoxComponents.Product,Images"
            );

            return _mapper.Map<GiftBoxResponse>(updatedGiftBox);
        }

        public async Task<bool> DeleteGiftBoxAsync(Guid id)
        {
            var giftBox = await _unitOfWork.GiftBoxRepository.GetFirstOrDefaultAsync(
                filter: g => g.Id == id && !g.IsDeleted
            );

            if (giftBox == null)
                return false;

            // Soft delete
            giftBox.IsDeleted = true;
            giftBox.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.GiftBoxRepository.Update(giftBox);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
