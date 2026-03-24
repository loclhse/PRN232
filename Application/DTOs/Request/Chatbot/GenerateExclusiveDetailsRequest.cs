namespace Application.DTOs.Request.Chatbot
{
    public class GenerateExclusiveDetailsRequest
    {
        // URL/đường dẫn tương đối tới ảnh trong thư mục wwwroot/images/custom-baskets/temp
        public string RelativeImagePath { get; set; } = string.Empty;

        // Prompt người dùng muốn OpenRouter custom tiếp
        public string UserPrompt { get; set; } = string.Empty;
    }
}

