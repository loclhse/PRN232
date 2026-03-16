namespace Application.DTOs.Response.InventoryTransaction
{
    public class InventoryTransactionResponse
    {
        public Guid Id { get; set; }

        public Guid InventoryId { get; set; }

        public string? InventoryProductName { get; set; }

        public int QuantityChange { get; set; }

        public string TransactionType { get; set; } = string.Empty; // Import, Sale, Return, Damage, Transfer

        public string? ReferenceId { get; set; } // OrderId, ImportReceiptId

        public string? Note { get; set; }

        public string? CreatedBy { get; set; } // Username or StaffId

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
