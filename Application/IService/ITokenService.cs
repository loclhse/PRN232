using Application.DTOs.Response;
using Domain.Entities;
using System.Security.Claims;

namespace Application.IService
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
