namespace Application.DTOs.Request
{
    public class CreateImageRequest
    {
        public string Url { get; set; } = string.Empty;
        public bool IsMain { get; set; }
        public int SortOrder { get; set; }

        // Cho phép null vì ảnh có thể thuộc về Product HOẶC GiftBox
        public Guid? ProductId { get; set; }
        public Guid? GiftBoxId { get; set; }
    }
}