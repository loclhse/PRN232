namespace Application.DTOs.Request.Chatbot
{
    public class ConfirmCustomBasketRequest
    {
        public string PreviewImageUrl { get; set; } = string.Empty;
        public List<CustomBasketProductItem> Products { get; set; } = new();
    }
}
