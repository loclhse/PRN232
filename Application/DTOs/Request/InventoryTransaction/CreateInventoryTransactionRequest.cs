using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Request.InventoryTransaction
{
    public class CreateInventoryTransactionRequest
    {
        [Required(ErrorMessage = "Inventory ID is required")]
        public Guid InventoryId { get; set; }

        [Required(ErrorMessage = "Quantity change is required")]
        public int QuantityChange { get; set; }

        [Required(ErrorMessage = "Transaction type is required")]
        [MaxLength(50, ErrorMessage = "Transaction type must not exceed 50 characters")]
        public string TransactionType { get; set; } = string.Empty; // Import, Sale, Return, Damage, Transfer

        [MaxLength(50, ErrorMessage = "Reference ID must not exceed 50 characters")]
        public string? ReferenceId { get; set; } // OrderId, ImportReceiptId

        public string? Note { get; set; }

        [MaxLength(100, ErrorMessage = "CreatedBy must not exceed 100 characters")]
        public string? CreatedBy { get; set; } // Username or StaffId
    }
}
