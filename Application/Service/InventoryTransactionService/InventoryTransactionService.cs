using Application.DTOs.Request.InventoryTransaction;
using Application.DTOs.Response.InventoryTransaction;
using AutoMapper;
using Domain.Entities;
using Domain.IUnitOfWork;
using InventoryTransactionEntity = Domain.Entities.InventoryTransaction;

namespace Application.Service.InventoryTransactionService
{
    public class InventoryTransactionService : IInventoryTransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public InventoryTransactionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// L?y t?t c? inventory transactions (không bao g?m nh?ng cái ?ã xóa)
        /// </summary>
        public async Task<IEnumerable<InventoryTransactionResponse>> GetAllTransactionsAsync()
        {
            var transactions = await _unitOfWork.Repository<InventoryTransactionEntity>().FindAsync(
                filter: t => !t.IsDeleted,
                includeProperties: "Inventory,Inventory.Product"
            );
            return _mapper.Map<IEnumerable<InventoryTransactionResponse>>(transactions);
        }

        /// <summary>
        /// L?y inventory transaction theo ID
        /// </summary>
        public async Task<InventoryTransactionResponse?> GetTransactionByIdAsync(Guid id)
        {
            var transaction = await _unitOfWork.Repository<InventoryTransactionEntity>().GetFirstOrDefaultAsync(
                filter: t => t.Id == id && !t.IsDeleted,
                includeProperties: "Inventory,Inventory.Product"
            );
            return _mapper.Map<InventoryTransactionResponse>(transaction);
        }

        /// <summary>
        /// L?y danh sách transactions theo Inventory ID
        /// </summary>
        public async Task<IEnumerable<InventoryTransactionResponse>> GetTransactionsByInventoryIdAsync(Guid inventoryId)
        {
            var transactions = await _unitOfWork.Repository<InventoryTransactionEntity>().FindAsync(
                filter: t => t.InventoryId == inventoryId && !t.IsDeleted,
                includeProperties: "Inventory,Inventory.Product",
                orderBy: q => q.OrderByDescending(t => t.CreatedAt)
            );
            return _mapper.Map<IEnumerable<InventoryTransactionResponse>>(transactions);
        }

        /// <summary>
        /// L?y danh sách transactions theo transaction type
        /// </summary>
        public async Task<IEnumerable<InventoryTransactionResponse>> GetTransactionsByTypeAsync(string transactionType)
        {
            var transactions = await _unitOfWork.Repository<InventoryTransactionEntity>().FindAsync(
                filter: t => t.TransactionType == transactionType && !t.IsDeleted,
                includeProperties: "Inventory,Inventory.Product",
                orderBy: q => q.OrderByDescending(t => t.CreatedAt)
            );
            return _mapper.Map<IEnumerable<InventoryTransactionResponse>>(transactions);
        }

        /// <summary>
        /// T?o m?i inventory transaction
        /// </summary>
        public async Task<InventoryTransactionResponse> CreateTransactionAsync(CreateInventoryTransactionRequest request)
        {
            // Ki?m tra xem InventoryId có t?n t?i không
            var inventory = await _unitOfWork.Repository<Inventory>().GetByIdAsync(request.InventoryId);
            if (inventory == null)
                throw new KeyNotFoundException($"Inventory with ID '{request.InventoryId}' not found.");

            var transaction = _mapper.Map<InventoryTransactionEntity>(request);
            transaction.Id = Guid.NewGuid();
            transaction.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.Repository<InventoryTransactionEntity>().AddAsync(transaction);
            await _unitOfWork.SaveChangesAsync();

            var createdTransaction = await _unitOfWork.Repository<InventoryTransactionEntity>().GetFirstOrDefaultAsync(
                filter: t => t.Id == transaction.Id,
                includeProperties: "Inventory,Inventory.Product"
            );

            return _mapper.Map<InventoryTransactionResponse>(createdTransaction);
        }

        /// <summary>
        /// C?p nh?t inventory transaction
        /// </summary>
        public async Task<InventoryTransactionResponse?> UpdateTransactionAsync(Guid id, UpdateInventoryTransactionRequest request)
        {
            var transaction = await _unitOfWork.Repository<InventoryTransactionEntity>().GetFirstOrDefaultAsync(
                filter: t => t.Id == id && !t.IsDeleted,
                includeProperties: "Inventory,Inventory.Product"
            );

            if (transaction == null)
                return null;

            // Map d? li?u c?p nh?t
            _mapper.Map(request, transaction);
            transaction.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<InventoryTransactionEntity>().Update(transaction);
            await _unitOfWork.SaveChangesAsync();

            // L?y l?i d? li?u m?i nh?t ?? tr? v?
            var updatedTransaction = await _unitOfWork.Repository<InventoryTransactionEntity>().GetFirstOrDefaultAsync(
                filter: t => t.Id == id,
                includeProperties: "Inventory,Inventory.Product"
            );

            return _mapper.Map<InventoryTransactionResponse>(updatedTransaction);
        }

        /// <summary>
        /// Xóa inventory transaction (soft delete)
        /// </summary>
        public async Task<bool> DeleteTransactionAsync(Guid id)
        {
            var transaction = await _unitOfWork.Repository<InventoryTransactionEntity>().GetByIdAsync(id);

            if (transaction == null || transaction.IsDeleted)
                return false;

            transaction.IsDeleted = true;
            transaction.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<InventoryTransactionEntity>().Update(transaction);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// L?y danh sách transactions theo reference ID
        /// </summary>
        public async Task<IEnumerable<InventoryTransactionResponse>> GetTransactionsByReferenceIdAsync(string referenceId)
        {
            var transactions = await _unitOfWork.Repository<InventoryTransactionEntity>().FindAsync(
                filter: t => t.ReferenceId == referenceId && !t.IsDeleted,
                includeProperties: "Inventory,Inventory.Product",
                orderBy: q => q.OrderByDescending(t => t.CreatedAt)
            );
            return _mapper.Map<IEnumerable<InventoryTransactionResponse>>(transactions);
        }
    }
}
