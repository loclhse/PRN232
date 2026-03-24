using System.Text.Json.Serialization;

namespace Application.DTOs.Request.Chatbot
{
    public class ProductSearchCriteria
    {
        [JsonPropertyName("needProductSearch")]
        public bool NeedProductSearch { get; set; } = false;

        [JsonPropertyName("products")]
        public List<string> Products { get; set; } = new();

        [JsonPropertyName("attributes")]
        public List<string> Attributes { get; set; } = new();

        [JsonPropertyName("occasion")]
        public string Occasion { get; set; } = string.Empty;

        [JsonPropertyName("recipient")]
        public string Recipient { get; set; } = string.Empty;

        [JsonPropertyName("minPrice")]
        public decimal? MinPrice { get; set; }

        [JsonPropertyName("maxPrice")]
        public decimal? MaxPrice { get; set; }

        [JsonPropertyName("sortPrice")]
        public string? SortPrice { get; set; }

        [JsonPropertyName("origin")]
        public List<string> Origin { get; set; } = new();

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
    }
}
