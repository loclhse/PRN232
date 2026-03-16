using Application.Service.InventoryTransactionService;
using Application.DTOs.Request.InventoryTransaction;
using Application.DTOs.Response.InventoryTransaction;
using Microsoft.AspNetCore.Mvc;

namespace PRN2322.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryTransactionsController : ControllerBase
    {
        private readonly IInventoryTransactionService _service;

        public InventoryTransactionsController(IInventoryTransactionService service)
        {
            _service = service;
        }

        /// <summary>
        /// L?y t?t c? inventory transactions
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryTransactionResponse>>> GetAllTransactions()
        {
            try
            {
                var transactions = await _service.GetAllTransactionsAsync();
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// L?y inventory transaction theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<InventoryTransactionResponse>> GetTransactionById(Guid id)
        {
            try
            {
                var transaction = await _service.GetTransactionByIdAsync(id);
                if (transaction == null)
                    return NotFound(new { message = $"Transaction with ID '{id}' not found." });
                
                return Ok(transaction);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// L?y danh sách transactions theo Inventory ID
        /// </summary>
        [HttpGet("byInventory/{inventoryId}")]
        public async Task<ActionResult<IEnumerable<InventoryTransactionResponse>>> GetTransactionsByInventoryId(Guid inventoryId)
        {
            try
            {
                var transactions = await _service.GetTransactionsByInventoryIdAsync(inventoryId);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// L?y danh sách transactions theo transaction type
        /// </summary>
        [HttpGet("byType/{transactionType}")]
        public async Task<ActionResult<IEnumerable<InventoryTransactionResponse>>> GetTransactionsByType(string transactionType)
        {
            try
            {
                var transactions = await _service.GetTransactionsByTypeAsync(transactionType);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// L?y danh sách transactions theo reference ID
        /// </summary>
        [HttpGet("byReference/{referenceId}")]
        public async Task<ActionResult<IEnumerable<InventoryTransactionResponse>>> GetTransactionsByReferenceId(string referenceId)
        {
            try
            {
                var transactions = await _service.GetTransactionsByReferenceIdAsync(referenceId);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// T?o m?i inventory transaction
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<InventoryTransactionResponse>> CreateTransaction([FromBody] CreateInventoryTransactionRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var transaction = await _service.CreateTransactionAsync(request);
                return CreatedAtAction(nameof(GetTransactionById), new { id = transaction.Id }, transaction);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// C?p nh?t inventory transaction
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<InventoryTransactionResponse>> UpdateTransaction(Guid id, [FromBody] UpdateInventoryTransactionRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var transaction = await _service.UpdateTransactionAsync(id, request);
                if (transaction == null)
                    return NotFound(new { message = $"Transaction with ID '{id}' not found." });

                return Ok(transaction);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Xóa inventory transaction (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTransaction(Guid id)
        {
            try
            {
                var result = await _service.DeleteTransactionAsync(id);
                if (!result)
                    return NotFound(new { message = $"Transaction with ID '{id}' not found or already deleted." });

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
