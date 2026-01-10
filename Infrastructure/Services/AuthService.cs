using Application.DTOs;
using Application.IService;
using Domain.Entities;
using Domain.IUnitOfWork;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;

        public AuthService(IUnitOfWork unitOfWork, ITokenService tokenService, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _configuration = configuration;
        }

        public async Task<TokenModel?> LoginWithGoogle(string credential)
        {
            try
            {
                var clientId = _configuration["Google:ClientId"] ?? throw new InvalidOperationException("Google ClientId is not configured.");
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string> { clientId }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(credential, settings);

                var userRepository = _unitOfWork.Repository<User>();
                var user = await userRepository.GetFirstOrDefaultAsync(
                    u => u.Email == payload.Email,
                    includeProperties: "Role");

                if (user == null)
                {
                    // Create new user if not exists
                    user = new User
                    {
                        Email = payload.Email,
                        FullName = payload.Name,
                        Username = payload.Email, // Use email as username for Google login
                        RoleId = 3, // Default to Customer
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        PasswordHash = Guid.NewGuid().ToString() // Dummy password for Google users
                    };

                    await userRepository.AddAsync(user);
                }

                var accessToken = _tokenService.GenerateAccessToken(user);
                var refreshToken = _tokenService.GenerateRefreshToken();

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

                // No need to call userRepository.Update if tracked
                await _unitOfWork.SaveChangesAsync();

                return new TokenModel
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<TokenModel?> Login(LoginRequest request)
        {
            var userRepository = _unitOfWork.Repository<User>();
            var user = await userRepository.GetFirstOrDefaultAsync(
                u => u.Email == request.Email,
                includeProperties: "Role");

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return null;
            }

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _unitOfWork.SaveChangesAsync();

            return new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<bool> Register(RegisterRequest request)
        {
            var userRepository = _unitOfWork.Repository<User>();
            
            // Check if user already exists
            var existingUser = await userRepository.GetFirstOrDefaultAsync(u => u.Username == request.Username || u.Email == request.Email);
            if (existingUser != null) return false;

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                FullName = request.FullName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Phone = request.Phone,
                Address = request.Address,
                RoleId = 3, // Default role: Customer
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await userRepository.AddAsync(user);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<TokenModel?> RefreshToken(TokenModel tokenModel)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(tokenModel.AccessToken);
            if (principal == null) return null;

            var email = principal.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return null;

            var userRepository = _unitOfWork.Repository<User>();
            var user = await userRepository.GetFirstOrDefaultAsync(
                u => u.Email == email,
                includeProperties: "Role");

            if (user == null || user.RefreshToken != tokenModel.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return null;

            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await _unitOfWork.SaveChangesAsync();

            return new TokenModel
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }
    }
}
