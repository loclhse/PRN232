using Application.DTOs.Request.Chatbot;
using Application.DTOs.Response.Chatbot;
using Application.Service.Chatbot;
using Domain.Entities;
using Domain.IUnitOfWork;
using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Infrastructure.Core
{
    public class ChatbotService : IChatbotService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;

        public ChatbotService(IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        public async Task<ChatMessageResponse> GetChatResponseAsync(string userMessage)
        {
            var apiKey = _configuration["GoogleAI:ApiKey"];
            if (string.IsNullOrEmpty(apiKey) || apiKey == "YOUR_GOOGLE_AI_API_KEY")
            {
                return new ChatMessageResponse
                {
                    Response = "Xin lỗi, chức năng chatbot chưa được cấu hình. Vui lòng thêm GoogleAI:ApiKey vào cấu hình."
                };
            }

            var client = new Client(apiKey: apiKey);

            // 1. Extract keywords from user message
            var criteria = await ExtractKeywordsAsync(client, userMessage);

            // 2. Search products in database
            var products = await SearchProductsAsync(criteria);

            // 3. Generate final response with Gemini
            var response = await GenerateConsultationAsync(client, userMessage, products);

            return new ChatMessageResponse
            {
                Response = response,
                ProductSuggestions = products.Select(p => new ProductSuggestion
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description ?? string.Empty,
                    Price = p.Price,
                    ImageUrl = p.Images?.FirstOrDefault(i => i.IsMain)?.Url
                }).ToList()
            };
        }

        private async Task<ProductSearchCriteria> ExtractKeywordsAsync(Client client, string userMessage)
        {
            var systemPrompt = """
Bạn là AI chuyên phân tích yêu cầu của khách hàng về sản phẩm quà tặng.

Nhiệm vụ: Đọc tin nhắn của khách hàng và trích xuất thông tin sau dưới dạng JSON:
{
  "products": ["danh sách sản phẩm/đồ vật khách hàng nhắc đến: gấu bông, kẹo, socola, rượu, hoa..."],
  "attributes": ["các thuộc tính: màu sắc, kích thước, chất liệu"],
  "occasion": "dịp tặng quà (sinh nhật, tết, valentine...)",
  "recipient": "người nhận (bạn nữ, bố mẹ, đồng nghiệp...)",
  "priceRange": "khoảng giá dạng số (vd: 500000 cho 500k, 1000000 cho 1 triệu)",
  "origin": ["xuất xứ/nguồn gốc: tách từng địa danh hoặc quốc gia thành phần tử riêng, viết thường. VD: 'bang cleveland Mỹ' -> ['cleveland','mỹ'], 'Nhật Bản' -> ['nhật bản'], 'Pháp, Ý' -> ['pháp','ý']"]
}

Nếu không có thông tin nào thì để rỗng [] hoặc "".

Ví dụ:
Input: "Tôi muốn tìm hộp quà có gấu bông màu hồng và socola cho bạn nữ, giá khoảng 500k"
Output: 
{"products":["gấu bông","socola"],"attributes":["màu hồng"],"occasion":"","recipient":"bạn nữ","priceRange":"500000","origin":[]}

Input: "Sản phẩm xuất xứ bang cleveland Mỹ"
Output:
{"products":[],"attributes":[],"occasion":"","recipient":"","priceRange":"","origin":["cleveland","mỹ"]}

Chỉ trả về JSON hợp lệ, không thêm văn bản nào khác.
""";

            var config = new GenerateContentConfig
            {
                SystemInstruction = new Content
                {
                    Parts = new List<Part> { new Part { Text = systemPrompt } }
                },
                Temperature = 0.1f
            };

            try
            {
                var response = await client.Models.GenerateContentAsync(
                    model: "gemini-2.5-flash-lite",
                    contents: userMessage,
                    config: config);

                var jsonText = response?.Candidates?[0]?.Content?.Parts?[0]?.Text?.Trim();
                if (string.IsNullOrEmpty(jsonText))
                    return new ProductSearchCriteria();

                if (jsonText.StartsWith("```"))
                {
                    var start = jsonText.IndexOf('{');
                    var end = jsonText.LastIndexOf('}') + 1;
                    if (start >= 0 && end > start)
                        jsonText = jsonText[start..end];
                }

                var criteria = JsonSerializer.Deserialize<ProductSearchCriteria>(jsonText);
                return criteria ?? new ProductSearchCriteria();
            }
            catch
            {
                return new ProductSearchCriteria();
            }
        }

        private async Task<List<Product>> SearchProductsAsync(ProductSearchCriteria criteria)
        {
            var allProducts = await _unitOfWork.ProductRepository.FindAsync(
                filter: null,
                includeProperties: "Category,Images",
                tracked: false);

            var query = allProducts.Where(p => p.IsActive);

         if (criteria.Products.Count > 0)
            {
                var keywords = criteria.Products
                    .Select(k => k.Trim().ToLowerInvariant())
                    .Where(k => !string.IsNullOrEmpty(k))
                    .ToList();

                if (keywords.Count > 0)
                {
                    query = query.Where(p =>
                        keywords.Any(kw =>
                            (p.Name + " " + (p.Description ?? "")).ToLowerInvariant().Contains(kw)));
                }
            }

            if (criteria.Attributes.Count > 0)
            {
                var attrs = criteria.Attributes
                    .Select(a => a.Trim().ToLowerInvariant())
                    .Where(a => !string.IsNullOrEmpty(a))
                    .ToList();

                if (attrs.Count > 0)
                {
                    query = query.Where(p =>
                        attrs.Any(attr =>
                            (p.Name + " " + (p.Description ?? "")).ToLowerInvariant().Contains(attr)));
                }
            }

            if (!string.IsNullOrWhiteSpace(criteria.PriceRange))
            {
                var priceStr = criteria.PriceRange.Replace("k", "000").Replace("K", "000")
                    .Replace("triệu", "000000").Replace("tr", "000000");
                if (decimal.TryParse(priceStr.Replace(",", "").Replace(".", ""), out var maxPrice) && maxPrice > 0)
                {
                    query = query.Where(p => p.Price <= maxPrice);
                }
            }

            if (criteria.Origin.Count > 0)
            {
                var originTerms = criteria.Origin
                    .Select(o => o.Trim().ToLowerInvariant())
                    .Where(o => !string.IsNullOrEmpty(o))
                    .ToList();

                if (originTerms.Count > 0)
                {
                    query = query.Where(p =>
                        originTerms.Any(term =>
                            (p.Name + " " + (p.Description ?? "")).ToLowerInvariant().Contains(term)));
                }
            }

            return query.Take(5).ToList();
        }

        private async Task<string> GenerateConsultationAsync(Client client, string userMessage, List<Product> products)
        {
            var productsContext = products.Count > 0
                ? string.Join("\n", products.Select((p, i) =>
                    $"- [{i + 1}] {p.Name} | Giá: {p.Price:N0} VNĐ | Mô tả: {(p.Description?.Length > 200 ? p.Description[..200] + "..." : p.Description ?? "")}"))
                : "Không tìm thấy sản phẩm phù hợp trong kho.";

            var systemPrompt = """
Bạn là nhân viên tư vấn thân thiện của HappyBox - cửa hàng chuyên về hộp quà tặng và sản phẩm quà tặng.

Nhiệm vụ: Dựa trên yêu cầu của khách hàng và danh sách sản phẩm tìm được, hãy tư vấn một cách thân thiện, chuyên nghiệp.

Quy tắc:
- Trả lời bằng tiếng Việt, thân thiện.
- Nếu có sản phẩm phù hợp: giới thiệu 1-3 sản phẩm nổi bật nhất và giải thích vì sao phù hợp.
- Nếu không có sản phẩm: gợi ý khách hàng thử mô tả khác hoặc liên hệ hotline.
- Giữ câu trả lời ngắn gọn, dễ đọc (2-4 câu).
""";

            var userContent = $"""
Yêu cầu khách hàng: "{userMessage}"

Danh sách sản phẩm tìm được:
{productsContext}

Hãy tư vấn cho khách hàng.
""";

            var config = new GenerateContentConfig
            {
                SystemInstruction = new Content
                {
                    Parts = new List<Part> { new Part { Text = systemPrompt } }
                },
                Temperature = 0.7f,
                MaxOutputTokens = 512
            };

            try
            {
                var response = await client.Models.GenerateContentAsync(
                    model: "gemini-2.5-flash-lite",
                    contents: userContent,
                    config: config);

                var text = response?.Candidates?[0]?.Content?.Parts?[0]?.Text?.Trim();
                return text ?? "Xin lỗi, tôi không thể xử lý được. Vui lòng thử lại.";
            }
            catch (Exception ex)
            {
                return $"Xin lỗi, đã có lỗi xảy ra: {ex.Message}";
            }
        }
    }
}
