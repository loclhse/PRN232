using Application.DTOs.Request.Auth;
using Application.DTOs.Request.Register;
using Application.DTOs.Response.Auth;
using Domain.Entities;

namespace Application.Service
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
