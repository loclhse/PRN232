using Application.DTOs.Response.Image;
using Application.DTOs.Response.Inventory;

namespace Application.DTOs.Response.Product
{
    public class ProductResponse
    {
        public Guid Id { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public Guid CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<ImageResponse>? Images { get; set; }
        public List<InventoryResponse>? Inventories { get; set; }
    }
}
