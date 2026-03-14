namespace Application.DTOs.Request.Chatbot
{
    public class CreateCustomBasketRequest
    {
        public string BasketImageUrl { get; set; } = string.Empty;
        public List<CustomBasketProductItem> Products { get; set; } = new();

    }

    public class CustomBasketProductItem
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; } = string.Empty;

    }
}

