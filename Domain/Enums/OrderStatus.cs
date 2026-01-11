namespace Domain.Enums
{
    public enum OrderStatus
    {
        Pending,        // Mới tạo
        Confirmed,      // Đã xác nhận
        Processing,     // Đang đóng gói/xử lý
        Shipping,       // Đang giao hàng
        Delivered,      // Đã giao thành công
        Cancelled,      // Đã hủy
        Returned        // Đã hoàn trả
    }
}
