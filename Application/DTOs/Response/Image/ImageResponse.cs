namespace Application.DTOs.Response.Image
{
    public class ImageResponse
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public bool IsMain { get; set; }
        public int SortOrder { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? GiftBoxId { get; set; }
    }
}