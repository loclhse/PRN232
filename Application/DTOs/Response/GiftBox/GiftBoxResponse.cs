using Application.DTOs.Response.Image;

namespace Application.DTOs.Response.GiftBox
{
    public class GiftBoxResponse
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public bool IsActive { get; set; }
        public bool IsCustom { get; set; }
        public Guid? UserId { get; set; }
        public Guid CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public Guid? GiftBoxComponentConfigId { get; set; }
        public string? ComponentConfigName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<ImageResponse>? Images { get; set; }
        public List<BoxComponentResponse>? BoxComponents { get; set; }
    }

    public class BoxComponentResponse
    {
        public Guid Id { get; set; }
        public Guid GiftBoxId { get; set; }
        public Guid ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductSKU { get; set; }
        public decimal ProductPrice { get; set; }
        public int Quantity { get; set; }
    }
}
