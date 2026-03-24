using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Request.InventoryTransaction
{
    public class UpdateInventoryTransactionRequest
    {
        [Required(ErrorMessage = "Quantity change is required")]
        public int QuantityChange { get; set; }

        [Required(ErrorMessage = "Transaction type is required")]
        [MaxLength(50, ErrorMessage = "Transaction type must not exceed 50 characters")]
        public string TransactionType { get; set; } = string.Empty;

        [MaxLength(50, ErrorMessage = "Reference ID must not exceed 50 characters")]
        public string? ReferenceId { get; set; }

        public string? Note { get; set; }

        [MaxLength(100, ErrorMessage = "CreatedBy must not exceed 100 characters")]
        public string? CreatedBy { get; set; }
    }
}
