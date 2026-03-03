namespace Application.DTOs.Response.Chatbot
{
    public class ChatMessageResponse
    {
        public string Response { get; set; } = string.Empty;
        public List<ProductSuggestion>? ProductSuggestions { get; set; }
    }

    public class ProductSuggestion
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
    }
}
