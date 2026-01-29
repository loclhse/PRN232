using Application.DTOs.Request.Product;
using Application.DTOs.Response.Product;
using AutoMapper;
using Domain.Entities;
using Domain.IUnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Application.Service.Product
{
    using ProductEntity = Domain.Entities.Product;
    using CategoryEntity = Domain.Entities.Category;

    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductResponse>> GetAllProductsAsync()
        {
            var products = await _unitOfWork.ProductRepository.FindAsync(
                filter: p => !p.IsDeleted,
                includeProperties: "Category"
            );

            return _mapper.Map<IEnumerable<ProductResponse>>(products);
        }

        public async Task<ProductResponse?> GetProductByIdAsync(Guid id)
        {
            var product = await _unitOfWork.ProductRepository.GetFirstOrDefaultAsync(
                filter: p => p.Id == id && !p.IsDeleted,
                includeProperties: "Category"
            );

            if (product == null)
                return null;

            return _mapper.Map<ProductResponse>(product);
        }

        public async Task<ProductResponse> CreateProductAsync(CreateProductRequest request)
        {
            // Check if SKU already exists
            var existingProduct = await _unitOfWork.ProductRepository.GetFirstOrDefaultAsync(
                filter: p => p.SKU == request.SKU && !p.IsDeleted
            );

            if (existingProduct != null)
                throw new InvalidOperationException($"Product with SKU '{request.SKU}' already exists.");

            // Check if Category exists
            var category = await _unitOfWork.Repository<CategoryEntity>().GetByIdAsync(request.CategoryId);
            if (category == null || category.IsDeleted)
                throw new InvalidOperationException($"Category with ID '{request.CategoryId}' not found.");

            var product = _mapper.Map<ProductEntity>(request);
            product.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.ProductRepository.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            // Reload with Category for response
            var createdProduct = await _unitOfWork.ProductRepository.GetFirstOrDefaultAsync(
                filter: p => p.Id == product.Id,
                includeProperties: "Category"
            );

            return _mapper.Map<ProductResponse>(createdProduct);
        }

        public async Task<ProductResponse?> UpdateProductAsync(Guid id, UpdateProductRequest request)
        {
            var product = await _unitOfWork.ProductRepository.GetFirstOrDefaultAsync(
                filter: p => p.Id == id && !p.IsDeleted
            );

            if (product == null)
                return null;

            // Check if SKU already exists for another product
            var existingProduct = await _unitOfWork.ProductRepository.GetFirstOrDefaultAsync(
                filter: p => p.SKU == request.SKU && p.Id != id && !p.IsDeleted
            );

            if (existingProduct != null)
                throw new InvalidOperationException($"Product with SKU '{request.SKU}' already exists.");

            // Check if Category exists
            var category = await _unitOfWork.Repository<CategoryEntity>().GetByIdAsync(request.CategoryId);
            if (category == null || category.IsDeleted)
                throw new InvalidOperationException($"Category with ID '{request.CategoryId}' not found.");

            _mapper.Map(request, product);
            product.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.SaveChangesAsync();

            // Reload with Category for response
            var updatedProduct = await _unitOfWork.ProductRepository.GetFirstOrDefaultAsync(
                filter: p => p.Id == id,
                includeProperties: "Category"
            );

            return _mapper.Map<ProductResponse>(updatedProduct);
        }

        public async Task<bool> DeleteProductAsync(Guid id)
        {
            var product = await _unitOfWork.ProductRepository.GetFirstOrDefaultAsync(
                filter: p => p.Id == id && !p.IsDeleted
            );

            if (product == null)
                return false;

            // Soft delete
            product.IsDeleted = true;
            product.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
