namespace Application.DTOs.Response.Cart
{
    public class CartItemResponse
    {
        public Guid Id { get; set; }
        public Guid CartId { get; set; }

        // Product info (nếu là Product)
        public Guid? ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductSKU { get; set; }
        public string? ProductImageUrl { get; set; }

        // GiftBox info (nếu là GiftBox)
        public Guid? GiftBoxId { get; set; }
        public string? GiftBoxName { get; set; }
        public string? GiftBoxCode { get; set; }
        public string? GiftBoxImageUrl { get; set; }

        // Loại item: "Product" hoặc "GiftBox"
        public string ItemType => ProductId.HasValue ? "Product" : "GiftBox";

        // Tên hiển thị (Product hoặc GiftBox)
        public string DisplayName => ProductId.HasValue ? ProductName ?? "" : GiftBoxName ?? "";

        // Hình ảnh hiển thị
        public string? DisplayImageUrl => ProductId.HasValue ? ProductImageUrl : GiftBoxImageUrl;

        public int Quantity { get; set; }

        // Đơn giá
        public decimal UnitPrice { get; set; }

        // Tổng tiền = Quantity * UnitPrice
        public decimal TotalPrice => Quantity * UnitPrice;

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
