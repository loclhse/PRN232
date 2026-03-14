using System.Text.Json.Serialization;

namespace Application.DTOs.Response.Chatbot
{
    public class OpenRouterResponse
    {
        [JsonPropertyName("choices")]
        public List<OpenRouterChoice> Choices { get; set; } = new();

        [JsonIgnore]
        public string GeneratedImageUrl =>
            Choices.FirstOrDefault()?.Message?.GetFirstImageUrl() ?? string.Empty;
    }
    public class OpenRouterChoice
    {
        [JsonPropertyName("message")]
        public OpenRouterMessage Message { get; set; } = new();
    }

    public class OpenRouterMessage
    {
        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;

        [JsonPropertyName("images")]
        public List<OpenRouterImagePart> Images { get; set; } = new();
        public string GetFirstImageUrl()
        {
            var first = Images?.FirstOrDefault();
            return first?.ImageUrl?.Url ?? string.Empty;
        }
    }
    public class OpenRouterImagePart
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("image_url")]
        public OpenRouterImageUrl? ImageUrl { get; set; }
    }

    public class OpenRouterImageUrl
    {
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;
    }
}
