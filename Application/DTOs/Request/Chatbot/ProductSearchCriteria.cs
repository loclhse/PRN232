using System.Text.Json.Serialization;

namespace Application.DTOs.Request.Chatbot
{
    public class ProductSearchCriteria
    {
        [JsonPropertyName("products")]
        public List<string> Products { get; set; } = new();

        [JsonPropertyName("attributes")]
        public List<string> Attributes { get; set; } = new();

        [JsonPropertyName("occasion")]
        public string Occasion { get; set; } = string.Empty;

        [JsonPropertyName("recipient")]
        public string Recipient { get; set; } = string.Empty;

        [JsonPropertyName("priceRange")]
        public string PriceRange { get; set; } = string.Empty;

        /// <summary>
        /// Xuất xứ / nguồn gốc (vd: "bang cleveland Mỹ" -> ["cleveland", "mỹ"])
        /// </summary>
        [JsonPropertyName("origin")]
        public List<string> Origin { get; set; } = new();
    }
}
