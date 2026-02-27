using Application.DTOs.Request.Inventory;
using Application.DTOs.Response;
using Application.DTOs.Response.Inventory;
using Application.Service.InventoryService;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PRN2322.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoriesController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoriesController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        /// <summary>
        /// L?y danh sách t?t c? inventory
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<InventoryResponse>>>> GetAllInventories()
        {
            try
            {
                var inventories = await _inventoryService.GetAllInventoriesAsync();
                return Ok(ApiResponse<IEnumerable<InventoryResponse>>.SuccessResponse(inventories, "Inventories retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<InventoryResponse>>.FailureResponse("An error occurred while retrieving inventories.", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// L?y thông tin inventory theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<InventoryResponse>>> GetInventoryById(Guid id)
        {
            try
            {
                var inventory = await _inventoryService.GetInventoryByIdAsync(id);

                if (inventory == null)
                {
                    return NotFound(ApiResponse<InventoryResponse>.FailureResponse("Inventory not found."));
                }

                return Ok(ApiResponse<InventoryResponse>.SuccessResponse(inventory, "Inventory retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<InventoryResponse>.FailureResponse("An error occurred while retrieving inventory.", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// L?y inventory theo ProductId
        /// </summary>
        [HttpGet("product/{productId}")]
        public async Task<ActionResult<ApiResponse<InventoryResponse>>> GetInventoryByProductId(Guid productId)
        {
            try
            {
                var inventory = await _inventoryService.GetInventoryByProductIdAsync(productId);

                if (inventory == null)
                {
                    return NotFound(ApiResponse<InventoryResponse>.FailureResponse("Inventory for this product not found."));
                }

                return Ok(ApiResponse<InventoryResponse>.SuccessResponse(inventory, "Inventory retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<InventoryResponse>.FailureResponse("An error occurred while retrieving inventory.", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// L?y inventory theo tr?ng thái
        /// </summary>
        [HttpGet("status/{status}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<InventoryResponse>>>> GetInventoriesByStatus(InventoryStatus status)
        {
            try
            {
                var inventories = await _inventoryService.GetInventoriesByStatusAsync(status);
                return Ok(ApiResponse<IEnumerable<InventoryResponse>>.SuccessResponse(inventories, $"Inventories with status '{status}' retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<InventoryResponse>>.FailureResponse("An error occurred while retrieving inventories by status.", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// T?o m?i inventory
        /// </summary>
        [HttpPost]
        //[Authorize]
        public async Task<ActionResult<ApiResponse<InventoryResponse>>> CreateInventory([FromBody] CreateInventoryRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                return BadRequest(ApiResponse<InventoryResponse>.FailureResponse("Validation failed", errors));
            }

            try
            {
                var inventory = await _inventoryService.CreateInventoryAsync(request);
                return CreatedAtAction(
                    nameof(GetInventoryById), 
                    new { id = inventory.Id }, 
                    ApiResponse<InventoryResponse>.SuccessResponse(inventory, "Inventory created successfully")
                );
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<InventoryResponse>.FailureResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<InventoryResponse>.FailureResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<InventoryResponse>.FailureResponse("An error occurred while creating the inventory.", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// C?p nh?t thông tin inventory
        /// </summary>
        [HttpPut("{id}")]
        //[Authorize]
        public async Task<ActionResult<ApiResponse<InventoryResponse>>> UpdateInventory(Guid id, [FromBody] UpdateInventoryRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                return BadRequest(ApiResponse<InventoryResponse>.FailureResponse("Validation failed", errors));
            }

            try
            {
                var inventory = await _inventoryService.UpdateInventoryAsync(id, request);

                if (inventory == null)
                {
                    return NotFound(ApiResponse<InventoryResponse>.FailureResponse("Inventory not found."));
                }

                return Ok(ApiResponse<InventoryResponse>.SuccessResponse(inventory, "Inventory updated successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<InventoryResponse>.FailureResponse("An error occurred while updating the inventory.", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Xóa inventory (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        //[Authorize]
        public async Task<ActionResult<ApiResponse>> DeleteInventory(Guid id)
        {
            try
            {
                var result = await _inventoryService.DeleteInventoryAsync(id);

                if (!result)
                {
                    return NotFound(ApiResponse.FailureResponse("Inventory not found."));
                }

                return Ok(ApiResponse.SuccessResponse("Inventory deleted successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.FailureResponse("An error occurred while deleting inventory.", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// C?p nh?t s? l??ng inventory (c?ng ho?c tr?)
        /// ???c s? d?ng khi có giao d?ch nh? bán hàng, nh?p kho, v.v.
        /// </summary>
        [HttpPatch("{id}/quantity")]
        //[Authorize]
        public async Task<ActionResult<ApiResponse>> UpdateQuantity(Guid id, [FromBody] UpdateQuantityRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                return BadRequest(ApiResponse.FailureResponse("Validation failed", errors));
            }

            try
            {
                var result = await _inventoryService.UpdateQuantityAsync(id, request.QuantityChange);

                if (!result)
                {
                    return NotFound(ApiResponse.FailureResponse("Inventory not found."));
                }

                return Ok(ApiResponse.SuccessResponse("Quantity updated successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.FailureResponse("An error occurred while updating quantity.", new List<string> { ex.Message }));
            }
        }
    }

    /// <summary>
    /// Request model for updating inventory quantity
    /// </summary>
    public class UpdateQuantityRequest
    {
        public int QuantityChange { get; set; }
    }
}
