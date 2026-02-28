using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Request.Inventory;
using Application.DTOs.Response.Inventory;
using Domain.Enums;

namespace Application.Service.InventoryService
{
    public interface IInventoryService
    {
        /// <summary>
        /// Lấy tất cả inventory
        /// </summary>
        Task<IEnumerable<InventoryResponse>> GetAllInventoriesAsync();

        /// <summary>
        /// Lấy inventory theo ID
        /// </summary>
        Task<InventoryResponse?> GetInventoryByIdAsync(Guid id);

        /// <summary>
        /// Lấy inventory theo ProductId
        /// </summary>
        Task<InventoryResponse?> GetInventoryByProductIdAsync(Guid productId);

        /// <summary>
        /// Tạo mới inventory
        /// </summary>
        Task<InventoryResponse> CreateInventoryAsync(CreateInventoryRequest request);

        /// <summary>
        /// Cập nhật inventory
        /// </summary>
        Task<InventoryResponse?> UpdateInventoryAsync(Guid id, UpdateInventoryRequest request);

        /// <summary>
        /// Xóa inventory (soft delete)
        /// </summary>
        Task<bool> DeleteInventoryAsync(Guid id);

        /// <summary>
        /// Lấy inventory theo trạng thái
        /// </summary>
        Task<IEnumerable<InventoryResponse>> GetInventoriesByStatusAsync(InventoryStatus status);

        /// <summary>
        /// Cập nhật số lượng inventory (cộng hoặc trừ)
        /// </summary>
        Task<bool> UpdateQuantityAsync(Guid inventoryId, int quantityChange);
    }
}
