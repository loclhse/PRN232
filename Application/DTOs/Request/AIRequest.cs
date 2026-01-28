namespace Application.DTOs.Request
{
    public class AIRequest
    {
        public string BasketImageUrl { get; set; } = string.Empty;
        public List<ProductItemRequest> Products { get; set; } = new();
    }

    public class ProductItemRequest
    {
        public string ProductImageUrl { get; set; } = string.Empty;
        public string FruitName { get; set; } = string.Empty;
    }
}
