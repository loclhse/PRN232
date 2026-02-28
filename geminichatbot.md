workflow:
1.User gửi tin nhắn: "Tôi muốn tìm một hộp quà cho bạn nữ, có gấu bông và kẹo."
2.Backend nhận tin: Gửi tin nhắn này sang Gemini kèm theo một System Instruction (Chỉ dẫn hệ thống).
3.Gemini trích xuất ý định (Intent Extraction): Gemini sẽ không trả lời ngay mà sẽ phân tích: "User đang tìm: GiftBox, thuộc tính: gấu bông, kẹo hoặc tìm 1 product có giá theo yêu cầu hoặc nguồn gốc của sản phầm mà user muốn hướng tới dựa vào field description trongtrong product ".
4.Tìm kiếm dữ liệu (Search): Backend lấy các từ khóa đó tìm trong Database để lấy ra 5.Top 3-5 sản phẩm khớp nhất.
6.Gemini tổng hợp câu trả lời (Final Response): Backend gửi lại cho Gemini: "Đây là thông tin 3 sản phẩm tìm được: [Sản phẩm A, B, C]. Hãy tư vấn cho user."
7.Chatbot phản hồi: "Chào bạn, dựa trên sở thích của bạn, mình gợi ý Hộp quà 'Ngọt Ngào' có gấu bông và kẹo dâu rất phù hợp..."

#yêu cầu Gemini phân tích câu nói của user và trả về kết quả có cấu trúc (Structured Output), tức là JSON thay vì văn bản tự do.

#Gemini cần phân biệt được:
Danh từ sản phẩm: "gấu bông", "kẹo", "rượu vang", "socola"
Thuộc tính: "màu hồng", "size lớn", "cao cấp"
Ngữ cảnh: "cho bạn nữ", "dịp sinh nhật", "giá rẻ"

# đây là hàm để extract keyword:
public async Task<ProductSearchCriteria> ExtractKeywords(string userMessage)
{
    var systemPrompt = @"
Bạn là AI chuyên phân tích yêu cầu của khách hàng về sản phẩm quà tặng.

Nhiệm vụ: Đọc tin nhắn của khách hàng và trích xuất thông tin sau dưới dạng JSON:
{
  ""products"": [""danh sách sản phẩm khách hàng nhắc đến""],
  ""attributes"": [""các thuộc tính như màu sắc, kích thước, chất liệu""],
  ""occasion"": ""dịp tặng quà (sinh nhật, tết, valentine...)"",
  ""recipient"": ""người nhận (bạn nữ, bố mẹ, đồng nghiệp...)"",
  ""priceRange"": ""khoảng giá (rẻ/trung bình/cao cấp)""
}

Ví dụ:
Input: ""Tôi muốn tìm hộp quà có gấu bông màu hồng và socola cho bạn nữ, giá khoảng 500k""
Output: 
{
  ""products"": [""gấu bông"", ""socola""],
  ""attributes"": [""màu hồng""],
  ""occasion"": """",
  ""recipient"": ""bạn nữ"",
  ""priceRange"": ""500000""
}

Chỉ trả về JSON, không thêm văn bản nào khác.
";

    var model = new GenerativeModel(apiKey: _configuration["GoogleAI:ApiKey"]);
    
    var response = await model.GenerateContentAsync(new[]
    {
        new Content("system", systemPrompt),
        new Content("user", userMessage)
    });

    var jsonResult = response.Text; // Gemini trả về chuỗi JSON
    var criteria = JsonSerializer.Deserialize<ProductSearchCriteria>(jsonResult);
    
    return criteria;
}

#responsedto:
public class ProductSearchCriteria
{
    public List<string> Products { get; set; } = new();
    public List<string> Attributes { get; set; } = new();
    public string Occasion { get; set; } = string.Empty;
    public string Recipient { get; set; } = string.Empty;
    public string PriceRange { get; set; } = string.Empty;
}

#Sử dụng kết quả để tìm kiếm trong Database:
public async Task<List<Product>> SearchProducts(ProductSearchCriteria criteria)
{
    var query = _unitOfWork.ProductRepository.GetAll();

    // Tìm kiếm theo tên sản phẩm hoặc mô tả
    if (criteria.Products.Any())
    {
        // Tạo điều kiện OR: tìm sản phẩm có chứa BẤT KỲ từ khóa nào
        query = query.Where(p => 
            criteria.Products.Any(keyword => 
                p.Name.Contains(keyword) || 
                p.Description.Contains(keyword)
            )
        );
    }

    // Lọc theo giá (nếu có)
    if (!string.IsNullOrEmpty(criteria.PriceRange))
    {
        if (decimal.TryParse(criteria.PriceRange, out var maxPrice))
        {
            query = query.Where(p => p.Price <= maxPrice);
        }
    }

    return await query.Take(5).ToListAsync(); // Lấy tối đa 5 sản phẩm
}