

Ý tưởng nâng cấp Chatbot tìm kiếm theo giá và sắp xếp:

1. Cập nhật DTO ProductSearchCriteria:
- Thêm `decimal? MinPrice`: Giá thấp nhất khách muốn.
- Thêm `decimal? MaxPrice`: Giá cao nhất khách muốn.
- Thêm `string? SortPrice`: Hướng sắp xếp ("asc" cho rẻ nhất, "desc" cho đắt nhất).

2. Cải tiến Prompt trong ExtractKeywordsAsync:
Yêu cầu AI trích xuất giá trị số thay vì chuỗi văn bản:
- "dưới 500k" => { "maxPrice": 500000, "minPrice": null }
- "từ 200k tới 1 triệu" => { "minPrice": 200000, "maxPrice": 1000000 }
- "chỉ có 500k thì mua món nào" => { "maxPrice": 500000, "sortPrice": "desc" } (Giải thích: Khi khách hỏi "mua món nào" với ngân sách cố định, AI nên hiểu là khách muốn tìm món tốt nhất/sát giá đó nhất, nên dùng desc).
- "rẻ nhất", "giá thấp nhất" => { "sortPrice": "asc" }
- "đắt nhất", "cao cấp nhất", "sang trọng nhất" => { "sortPrice": "desc" }

3. Cập nhật Logic SearchProductsAsync:
- Áp dụng lọc: `query = query.Where(p => p.Price >= criteria.MinPrice)` nếu có MinPrice.
- Áp dụng lọc: `query = query.Where(p => p.Price <= criteria.MaxPrice)` nếu có MaxPrice.
- Áp dụng sắp xếp: `query = query.OrderBy(p => p.Price)` nếu sortPrice == "asc".
- Áp dụng sắp xếp: `query = query.OrderByDescending(p => p.Price)` nếu sortPrice == "desc".

Lợi ích:
- Chatbot hiểu được các yêu cầu phức tạp về ngân sách của khách hàng.
- Kết quả trả về chính xác hơn (ví dụ: khách hỏi "quà dưới 200k rẻ nhất" sẽ ra đúng sản phẩm rẻ nhất trong tầm giá).
- Giảm thiểu việc parse chuỗi thủ công trong code C#, tận dụng khả năng xử lý ngôn ngữ của AI để đưa ra con số chính xác.
--------------------------------------------------------------
sau khi request: 
{
  "message": "tôi muốn mua vài món đồ"
}

thì response thành :
{
  "success": true,
  "message": "Chat response retrieved successfully",
  "data": {
    "response": "Chào bạn, rất vui được hỗ trợ bạn tại HappyBox! 😊\n\nVì chưa tìm được món đồ ưng ý với mô tả hiện tại, bạn có thể thử miêu tả chi tiết hơn về món quà bạn đang tìm kiếm không ạ? Hoặc nếu tiện, bạn có thể gọi đến hotline [Số điện thoại hotline] để chúng tôi có thể tư vấn trực tiếp và nhanh chóng nhất cho bạn nhé!",
    "productSuggestions": null
  },
  "errors": null
} thay vì sug