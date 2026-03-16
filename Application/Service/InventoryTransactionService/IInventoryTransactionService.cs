using Application.DTOs.Request.InventoryTransaction;
using Application.DTOs.Response.InventoryTransaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.InventoryTransactionService
{
    public interface IInventoryTransactionService
    {
        /// <summary>
        /// L?y t?t c? inventory transactions
        /// </summary>
        Task<IEnumerable<InventoryTransactionResponse>> GetAllTransactionsAsync();

        /// <summary>
        /// L?y inventory transaction theo ID
        /// </summary>
        Task<InventoryTransactionResponse?> GetTransactionByIdAsync(Guid id);

        /// <summary>
        /// L?y danh sách transactions theo Inventory ID
        /// </summary>
        Task<IEnumerable<InventoryTransactionResponse>> GetTransactionsByInventoryIdAsync(Guid inventoryId);

        /// <summary>
        /// L?y danh sách transactions theo transaction type
        /// </summary>
        Task<IEnumerable<InventoryTransactionResponse>> GetTransactionsByTypeAsync(string transactionType);

        /// <summary>
        /// T?o m?i inventory transaction
        /// </summary>
        Task<InventoryTransactionResponse> CreateTransactionAsync(CreateInventoryTransactionRequest request);

        /// <summary>
        /// C?p nh?t inventory transaction
        /// </summary>
        Task<InventoryTransactionResponse?> UpdateTransactionAsync(Guid id, UpdateInventoryTransactionRequest request);

        /// <summary>
        /// Xóa inventory transaction (soft delete)
        /// </summary>
        Task<bool> DeleteTransactionAsync(Guid id);

        /// <summary>
        /// L?y danh sách transactions theo reference ID
        /// </summary>
        Task<IEnumerable<InventoryTransactionResponse>> GetTransactionsByReferenceIdAsync(string referenceId);
    }
}
