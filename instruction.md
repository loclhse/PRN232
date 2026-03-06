- áp dụng atomix request cho CreateGiftBoxAsync(CreateGiftBoxRequest request)
+ Cấu trúc Request DTO đề xuất:
 {
  "name": "Hộp quà Tết Đoàn Viên",
  "description": "Hộp quà cao cấp dành cho gia đình",
  "basePrice": 500000,
  "categoryId": "...",
  "giftBoxComponentConfigId": "...",
  "items": [
    {
      "productId": "guid-sp-1",
      "quantity": 2
    },
    {
      "productId": "guid-sp-2",
      "quantity": 1
    }
  ]
}

- Cách xử lý trong Service (GiftBoxService):
1. Mở Transaction: Sử dụng IUnitOfWork để đảm bảo tính nguyên tử.
2. Tạo GiftBox: Lưu thông tin cơ bản của Box trước để lấy được Id.
3. Tạo BoxComponents:
 Duyệt qua danh sách items trong request.
 Kiểm tra xem ProductId có tồn tại và còn hàng (IsActive) không.
 Tạo các đối tượng BoxComponent liên kết GiftBoxId vừa tạo với ProductId.
4. Tính toán lại giá (Tùy chọn): Có thể cộng dồn BasePrice của Box + (Giá Product * Quantity) để ra giá tổng nếu cần.
5. Commit Transaction: Lưu tất cả vào Database.

- Images: do hộp quà có ảnh riêng, nên cho phép gửi kèm danh sách ImageUrls trong cùng API này.

- N-N Relationship: Vì BoxComponent là bảng trung gian có chứa thêm thông tin (Quantity), nên việc xử lý thủ công trong Service như trên là cách kiểm soát tốt nhất bằng cách trừ search inventory của product đó bằng product id sau đó trừ quantity của product đó trong inventory với quantity của product đó trong giftboxcomponent