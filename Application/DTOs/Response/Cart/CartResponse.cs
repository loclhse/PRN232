namespace Application.DTOs.Response.Cart
{
    public class CartResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;

        // Danh sách items trong giỏ
        public List<CartItemResponse> Items { get; set; } = new();

        // Tổng số lượng items
        public int TotalItems => Items.Sum(i => i.Quantity);

        // Số loại sản phẩm khác nhau
        public int UniqueItemsCount => Items.Count;

        // Tổng tiền của giỏ hàng
        public decimal SubTotal => Items.Sum(i => i.TotalPrice);

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
