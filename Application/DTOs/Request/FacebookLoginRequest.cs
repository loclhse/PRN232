namespace Application.DTOs.Request
{
    public class FacebookLoginRequest
    {
        public string AccessToken { get; set; } = string.Empty;
    }

    public class FacebookUserInfo
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public FacebookPicture? Picture { get; set; }
    }

    public class FacebookPicture
    {
        public FacebookPictureData? Data { get; set; }
    }

    public class FacebookPictureData
    {
        public string Url { get; set; } = string.Empty;
    }
}
