using Application.DTOs.Request;
using Application.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductResponse>> GetAllProductsAsync();
        Task<ProductResponse?> GetProductByIdAsync(Guid id);
        Task<ProductResponse> CreateProductAsync(CreateProductRequest request);
        Task<ProductResponse?> UpdateProductAsync(Guid id, UpdateProductRequest request);
        Task<bool> DeleteProductAsync(Guid id);
    }
}
