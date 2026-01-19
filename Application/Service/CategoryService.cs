using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.IService;
using AutoMapper;
using Domain.Entities;
using Domain.IUnitOfWork;

namespace Application.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryResponse>> GetAllCategoriesAsync()
        {
            var categories = await _unitOfWork.categoryRepository.FindAsync(
                filter: c => !c.IsDeleted,
                includeProperties: "ParentCategory"
            );

            return _mapper.Map<IEnumerable<CategoryResponse>>(categories);
        }

        public async Task<CategoryResponse?> GetCategoryByIdAsync(Guid id)
        {
            var category = await _unitOfWork.categoryRepository.GetFirstOrDefaultAsync(
                filter: c => c.Id == id && !c.IsDeleted,
                includeProperties: "ParentCategory"
            );

            if (category == null)
                return null;

            return _mapper.Map<CategoryResponse>(category);
        }

        public async Task<CategoryResponse> CreateCategoryAsync(CreateCategoryRequest request)
        {
            // Ensure category name is unique (not deleted)
            var existing = await _unitOfWork.categoryRepository.GetFirstOrDefaultAsync(
                filter: c => c.Name == request.Name && !c.IsDeleted
            );

            if (existing != null)
                throw new InvalidOperationException($"Category with name '{request.Name}' already exists.");

            // Validate parent
            if (request.ParentId.HasValue)
            {
                var parent = await _unitOfWork.categoryRepository.GetFirstOrDefaultAsync(
                    filter: c => c.Id == request.ParentId.Value && !c.IsDeleted
                );

                if (parent == null)
                    throw new InvalidOperationException($"Parent category with ID '{request.ParentId}' not found.");
            }

            var category = _mapper.Map<Category>(request);
            category.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.categoryRepository.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            var created = await _unitOfWork.categoryRepository.GetFirstOrDefaultAsync(
                filter: c => c.Id == category.Id,
                includeProperties: "ParentCategory"
            );

            return _mapper.Map<CategoryResponse>(created);
        }

        public async Task<CategoryResponse?> UpdateCategoryAsync(Guid id, UpdateCategoryRequest request)
        {
            var category = await _unitOfWork.categoryRepository.GetFirstOrDefaultAsync(
                filter: c => c.Id == id && !c.IsDeleted
            );

            if (category == null)
                return null;

            // Prevent self-parenting
            if (request.ParentId.HasValue && request.ParentId.Value == id)
                throw new InvalidOperationException("A category cannot be its own parent.");

            // Ensure unique name
            var existing = await _unitOfWork.categoryRepository.GetFirstOrDefaultAsync(
                filter: c => c.Name == request.Name && c.Id != id && !c.IsDeleted
            );

            if (existing != null)
                throw new InvalidOperationException($"Category with name '{request.Name}' already exists.");

            // Validate parent
            if (request.ParentId.HasValue)
            {
                var parent = await _unitOfWork.categoryRepository.GetFirstOrDefaultAsync(
                    filter: c => c.Id == request.ParentId.Value && !c.IsDeleted
                );

                if (parent == null)
                    throw new InvalidOperationException($"Parent category with ID '{request.ParentId}' not found.");
            }

            _mapper.Map(request, category);
            category.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.categoryRepository.Update(category);
            await _unitOfWork.SaveChangesAsync();

            var updated = await _unitOfWork.categoryRepository.GetFirstOrDefaultAsync(
                filter: c => c.Id == id,
                includeProperties: "ParentCategory"
            );

            return _mapper.Map<CategoryResponse>(updated);
        }

        public async Task<bool> DeleteCategoryAsync(Guid id)
        {
            var category = await _unitOfWork.categoryRepository.GetFirstOrDefaultAsync(
                filter: c => c.Id == id && !c.IsDeleted
            );

            if (category == null)
                return false;

            // Soft delete
            category.IsDeleted = true;
            category.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.categoryRepository.Update(category);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
