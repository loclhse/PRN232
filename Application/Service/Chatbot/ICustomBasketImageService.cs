using Application.DTOs.Request.Chatbot;

namespace Application.Service.Chatbot
{
    public interface ICustomBasketImageService
    {
        Task<string> GenerateCustomBasketAsync(CreateCustomBasketRequest request, Guid userId);
        Task<Guid> ConfirmCustomBasketAsync(ConfirmCustomBasketRequest request, Guid userId);
    }
}

