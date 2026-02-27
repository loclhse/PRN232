using Application.DTOs.Response;
using Domain.Entities;
using System.Security.Claims;

namespace Application.Service
{
    public interface ITokenService
    {
        string GenerateAccessToken(Domain.Entities.User user);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
