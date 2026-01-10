using Application.DTOs.Request;
using Application.DTOs.Response;
using Domain.Entities;

namespace Application.IService
{
    public interface IAuthService
    {
        Task<TokenModel?> LoginWithGoogle(string credential);
        Task<TokenModel?> Login(LoginRequest request);
        Task<bool> Register(RegisterRequest request);
        Task<bool> ForgotPassword(string email);
        Task<bool> ResetPassword(ResetPasswordWithOtpRequest request);
        Task<TokenModel?> RefreshToken(TokenModel tokenModel);
        Task<UserResponse?> GetProfile(string email);
    }
}
