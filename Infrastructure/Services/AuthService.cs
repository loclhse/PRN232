using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.IService;
using Domain.Entities;
using Domain.IUnitOfWork;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;

namespace Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly IMailService _mailService;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthService(IUnitOfWork unitOfWork, ITokenService tokenService, IConfiguration configuration, IMailService mailService, IMapper mapper, IDistributedCache cache, IHttpClientFactory httpClientFactory)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _configuration = configuration;
            _mailService = mailService;
            _mapper = mapper;
            _cache = cache;
            _httpClientFactory = httpClientFactory;
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

                var user = await _unitOfWork.UserRepository.GetFirstOrDefaultAsync(
                    u => u.Email == payload.Email,
                    includeProperties: "Role");

                if (user == null)
                {
                    // Create new user if not exists
                    user = _mapper.Map<User>(payload);
                    await _unitOfWork.UserRepository.AddAsync(user);
                }

                var accessToken = _tokenService.GenerateAccessToken(user);
                var refreshToken = _tokenService.GenerateRefreshToken();

                // Lưu Refresh Token vào Redis (Hết hạn sau 7 ngày)
                var cacheKey = $"refreshToken:{user.Email}";
                await _cache.SetStringAsync(cacheKey, refreshToken, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
                });

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

        public async Task<TokenModel?> LoginWithFacebook(string accessToken)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync($"https://graph.facebook.com/me?fields=id,name,email,picture&access_token={accessToken}");

                if (!response.IsSuccessStatusCode) return null;

                var content = await response.Content.ReadAsStringAsync();
                var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var facebookUser = System.Text.Json.JsonSerializer.Deserialize<Application.DTOs.Request.FacebookUserInfo>(content, options);

                if (facebookUser == null || string.IsNullOrEmpty(facebookUser.Email)) return null;

                var user = await _unitOfWork.UserRepository.GetFirstOrDefaultAsync(
                    u => u.Email == facebookUser.Email,
                    includeProperties: "Role");

                if (user == null)
                {
                    // Create new user if not exists
                    user = new User
                    {
                        Id = Guid.NewGuid(),
                        Email = facebookUser.Email,
                        FullName = facebookUser.Name,
                        Username = facebookUser.Email,
                        RoleId = Domain.Constants.RoleIds.Customer,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString())
                    };
                    await _unitOfWork.UserRepository.AddAsync(user);
                }

                var newAccessToken = _tokenService.GenerateAccessToken(user);
                var newRefreshToken = _tokenService.GenerateRefreshToken();

                var cacheKey = $"refreshToken:{user.Email}";
                await _cache.SetStringAsync(cacheKey, newRefreshToken, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
                });

                await _unitOfWork.SaveChangesAsync();

                return new TokenModel
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<TokenModel?> Login(LoginRequest request)
        {
            var user = await _unitOfWork.UserRepository.GetFirstOrDefaultAsync(
                u => u.Email == request.Email,
                includeProperties: "Role");

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return null;
            }

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            // Lưu Refresh Token vào Redis
            var cacheKey = $"refreshToken:{user.Email}";
            await _cache.SetStringAsync(cacheKey, refreshToken, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
            });

            await _unitOfWork.SaveChangesAsync();

            return new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<bool> Register(RegisterRequest request)
        {
            // Check if user already exists
            var existingUser = await _unitOfWork.UserRepository.GetFirstOrDefaultAsync(u => u.Username == request.Username || u.Email == request.Email);
            if (existingUser != null) return false;

            var user = _mapper.Map<User>(request);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            await _unitOfWork.UserRepository.AddAsync(user);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<bool> ForgotPassword(string email)
        {
            var user = await _unitOfWork.UserRepository.GetFirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return false;

            // Tạo mã OTP 6 chữ số
            var otpCode = new Random().Next(100000, 999999).ToString();
            
            // Lưu OTP vào Redis (Hết hạn sau 5 phút)
            var cacheKey = $"otp:{email}";
            await _cache.SetStringAsync(cacheKey, otpCode, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
            });

            var subject = "Your OTP Code - HappyBox";
            var body = $@"
                <div style='font-family: Arial, sans-serif; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
                    <h2 style='color: #4CAF50;'>Hello {user.FullName},</h2>
                    <p>You requested to reset your password. Use the OTP code below to proceed:</p>
                    <div style='font-size: 24px; font-weight: bold; background: #f4f4f4; padding: 10px; display: inline-block; letter-spacing: 5px;'>
                        {otpCode}
                    </div>
                    <p>This code will expire in <b>5 minutes</b>.</p>
                    <p>If you didn't request this, please ignore this email.</p>
                </div>";

            try
            {
                await _mailService.SendEmailAsync(user.Email, subject, body);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> ResetPassword(ResetPasswordWithOtpRequest request)
        {
            var user = await _unitOfWork.UserRepository.GetFirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null) return false;

            var cacheKey = $"otp:{request.Email}";
            var savedOtp = await _cache.GetStringAsync(cacheKey);

            if (savedOtp == null || savedOtp != request.Otp)
            {
                return false;
            }

            // Xóa OTP ngay sau khi dùng xong
            await _cache.RemoveAsync(cacheKey);
            
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<TokenModel?> RefreshToken(TokenModel tokenModel)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(tokenModel.AccessToken);
            if (principal == null) return null;

            var email = principal.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return null;

            var user = await _unitOfWork.UserRepository.GetFirstOrDefaultAsync(
                u => u.Email == email,
                includeProperties: "Role");

            var cacheKey = $"refreshToken:{email}";
            var savedRefreshToken = await _cache.GetStringAsync(cacheKey);

            if (user == null || savedRefreshToken != tokenModel.RefreshToken)
                return null;

            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            // Cập nhật Refresh Token mới vào Redis
            await _cache.SetStringAsync(cacheKey, newRefreshToken, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
            });

            await _unitOfWork.SaveChangesAsync();

            return new TokenModel
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }

        public async Task<UserResponse?> GetProfile(string email)
        {
            var user = await _unitOfWork.UserRepository.GetFirstOrDefaultAsync(
                u => u.Email == email,
                includeProperties: "Role");

            if (user == null) return null;

            return _mapper.Map<UserResponse>(user);
        }
    }
}
