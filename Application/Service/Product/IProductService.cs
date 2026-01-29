using Application.DTOs.Request.Product;
using Application.DTOs.Response.Product;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Service.Product
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
