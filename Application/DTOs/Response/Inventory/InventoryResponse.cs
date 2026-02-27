using Domain.Enums;

namespace Application.DTOs.Response.Inventory
{
    public class InventoryResponse
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public string? ProductName { get; set; }

        public int Quantity { get; set; }

        public int MinStockLevel { get; set; }

        public InventoryStatus Status { get; set; }

        public DateTime LastUpdated { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
