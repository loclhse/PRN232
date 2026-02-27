using Application.DTOs.Request.Inventory;
using Application.DTOs.Response.Inventory;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.IUnitOfWork;
using InventoryEntity = Domain.Entities.Inventory;

namespace Application.Service.InventoryService
{
    public class InventoryService : IInventoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public InventoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Lấy tất cả inventory (không bao gồm những cái đã xóa)
        /// </summary>
        public async Task<IEnumerable<InventoryResponse>> GetAllInventoriesAsync()
        {
            var inventories = await _unitOfWork.Repository<InventoryEntity>().FindAsync(
                filter: i => !i.IsDeleted,
                includeProperties: "Product,Transactions"
            );
            return _mapper.Map<IEnumerable<InventoryResponse>>(inventories);
        }

        /// <summary>
        /// Lấy inventory theo ID
        /// </summary>
        public async Task<InventoryResponse?> GetInventoryByIdAsync(Guid id)
        {
            var inventory = await _unitOfWork.Repository<InventoryEntity>().GetFirstOrDefaultAsync(
                filter: i => i.Id == id && !i.IsDeleted,
                includeProperties: "Product,Transactions"
            );
            return _mapper.Map<InventoryResponse>(inventory);
        }

        /// <summary>
        /// Lấy inventory theo ProductId
        /// </summary>
        public async Task<InventoryResponse?> GetInventoryByProductIdAsync(Guid productId)
        {
            var inventory = await _unitOfWork.Repository<InventoryEntity>().GetFirstOrDefaultAsync(
                filter: i => i.ProductId == productId && !i.IsDeleted,
                includeProperties: "Product,Transactions"
            );
            return _mapper.Map<InventoryResponse>(inventory);
        }

        /// <summary>
        /// Tạo mới inventory cho một sản phẩm
        /// </summary>
        public async Task<InventoryResponse> CreateInventoryAsync(CreateInventoryRequest request)
        {
            // Kiểm tra xem ProductId có tồn tại không
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(request.ProductId);
            if (product == null)
                throw new KeyNotFoundException($"Product with ID '{request.ProductId}' not found.");

            // Kiểm tra xem đã có inventory cho product này chưa
            var existing = await _unitOfWork.Repository<InventoryEntity>().GetFirstOrDefaultAsync(
                filter: i => i.ProductId == request.ProductId && !i.IsDeleted
            );

            if (existing != null)
                throw new InvalidOperationException($"Inventory for Product '{request.ProductId}' already exists.");

            var inventory = _mapper.Map<InventoryEntity>(request);
            inventory.Id = Guid.NewGuid();
            inventory.CreatedAt = DateTime.UtcNow;
            inventory.LastUpdated = DateTime.UtcNow;

            // Set status based on quantity
            inventory.Status = request.Quantity <= 0 ? InventoryStatus.OutOfStock : InventoryStatus.InStock;

            await _unitOfWork.Repository<InventoryEntity>().AddAsync(inventory);
            await _unitOfWork.SaveChangesAsync();

            var createdInventory = await _unitOfWork.Repository<InventoryEntity>().GetFirstOrDefaultAsync(
                filter: i => i.Id == inventory.Id,
                includeProperties: "Product,Transactions"
            );

            return _mapper.Map<InventoryResponse>(createdInventory);
        }

        /// <summary>
        /// Cập nhật thông tin inventory (số lượng, mức tồn kho tối thiểu)
        /// </summary>
        public async Task<InventoryResponse?> UpdateInventoryAsync(Guid id, UpdateInventoryRequest request)
        {
            var inventory = await _unitOfWork.Repository<InventoryEntity>().GetFirstOrDefaultAsync(
                filter: i => i.Id == id && !i.IsDeleted,
                includeProperties: "Product,Transactions"
            );

            if (inventory == null)
                return null;

            // Map dữ liệu cập nhật
            _mapper.Map(request, inventory);
            inventory.LastUpdated = DateTime.UtcNow;
            inventory.UpdatedAt = DateTime.UtcNow;

            // Cập nhật trạng thái dựa trên số lượng
            if (inventory.Quantity <= 0)
            {
                inventory.Status = InventoryStatus.OutOfStock;
            }
            else if (inventory.Quantity < inventory.MinStockLevel)
            {
                inventory.Status = InventoryStatus.LowStock;
            }
            else
            {
                inventory.Status = InventoryStatus.InStock;
            }

            _unitOfWork.Repository<InventoryEntity>().Update(inventory);
            await _unitOfWork.SaveChangesAsync();

            // Lấy lại dữ liệu mới nhất để trả về
            var updatedInventory = await _unitOfWork.Repository<InventoryEntity>().GetFirstOrDefaultAsync(
                filter: i => i.Id == id,
                includeProperties: "Product,Transactions"
            );

            return _mapper.Map<InventoryResponse>(updatedInventory);
        }

        /// <summary>
        /// Xóa inventory (soft delete)
        /// </summary>
        public async Task<bool> DeleteInventoryAsync(Guid id)
        {
            var inventory = await _unitOfWork.Repository<InventoryEntity>().GetByIdAsync(id);

            if (inventory == null || inventory.IsDeleted)
                return false;

            inventory.IsDeleted = true;
            inventory.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<InventoryEntity>().Update(inventory);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// Lấy danh sách inventory theo trạng thái
        /// </summary>
        public async Task<IEnumerable<InventoryResponse>> GetInventoriesByStatusAsync(InventoryStatus status)
        {
            var inventories = await _unitOfWork.Repository<InventoryEntity>().FindAsync(
                filter: i => i.Status == status && !i.IsDeleted,
                includeProperties: "Product,Transactions"
            );

            return _mapper.Map<IEnumerable<InventoryResponse>>(inventories);
        }

        /// <summary>
        /// Cập nhật số lượng inventory (cộng hoặc trừ)
        /// Được dùng khi có giao dịch (bán hàng, nhập kho, trả hàng, etc.)
        /// </summary>
        public async Task<bool> UpdateQuantityAsync(Guid inventoryId, int quantityChange)
        {
            var inventory = await _unitOfWork.Repository<InventoryEntity>().GetByIdAsync(inventoryId);

            if (inventory == null)
                return false;

            inventory.Quantity += quantityChange;
            inventory.LastUpdated = DateTime.UtcNow;
            inventory.UpdatedAt = DateTime.UtcNow;

            // Cập nhật trạng thái dựa trên số lượng
            if (inventory.Quantity <= 0)
            {
                inventory.Status = InventoryStatus.OutOfStock;
            }
            else if (inventory.Quantity < inventory.MinStockLevel)
            {
                inventory.Status = InventoryStatus.LowStock;
            }
            else
            {
                inventory.Status = InventoryStatus.InStock;
            }

            _unitOfWork.Repository<InventoryEntity>().Update(inventory);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }
    }
}
