using Application.DTOs;
using Domain.Entities;

namespace Application.IService
{
    public interface IAuthService
    {
        Task<TokenModel?> LoginWithGoogle(string credential);
        Task<TokenModel?> Login(LoginRequest request);
        Task<bool> Register(RegisterRequest request);
        Task<TokenModel?> RefreshToken(TokenModel tokenModel);
    }
}
