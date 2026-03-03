using Domain.Entities;
using Domain.IUnitOfWork;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using Application.Service;
using Application.DTOs.Request.Register;
using Application.DTOs.Request.Auth;
using Application.DTOs.Response.Auth;

namespace Infrastructure.Core
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

                credential = credential.Trim().Trim('\"');
                var payload = await GoogleJsonWebSignature.ValidateAsync(credential, settings);

                var user = await _unitOfWork.UserRepository.GetFirstOrDefaultAsync(
                    u => u.Email == payload.Email,
                    includeProperties: "Role");

                if (user == null)
                {
                   
                    user = _mapper.Map<User>(payload);
                    await _unitOfWork.UserRepository.AddAsync(user);
                }

                var accessToken = _tokenService.GenerateAccessToken(user);
                var refreshToken = _tokenService.GenerateRefreshToken();

             
                var cacheKey = $"refreshToken:{user.Email}";
                await _cache.SetStringAsync(cacheKey, refreshToken, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
                });

                await _unitOfWork.SaveChangesAsync();

                return new TokenModel
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    User = new UserAuthInfo
                    {
                        Id = user.Id,
                        Email = user.Email,
                        FullName = user.FullName,
                        Username = user.Username,
                        Phone = user.Phone ?? string.Empty,
                        Address = user.Address ?? string.Empty,
                        RoleName = user.Role?.RoleName.ToString() ?? "Customer",
                        IsActive = user.IsActive
                    }
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Google Login Error: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                throw;
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

           
            var cacheKey = $"refreshToken:{user.Email}";
            await _cache.SetStringAsync(cacheKey, refreshToken, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
            });

            await _unitOfWork.SaveChangesAsync();

            return new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = new UserAuthInfo
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    Username = user.Username,
                    Phone = user.Phone ?? string.Empty,
                    Address = user.Address ?? string.Empty,
                    RoleName = user.Role?.RoleName.ToString() ?? "Customer",
                    IsActive = user.IsActive
                }
            };
        }

        public async Task<bool> Register(RegisterRequest request)
        {
           
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

         
            var otpCode = new Random().Next(100000, 999999).ToString();
            
         
            var cacheKey = $"otp:{email}";
            try
            {
                await _cache.SetStringAsync(cacheKey, otpCode, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) 
                });
                
               
                var savedOtp = await _cache.GetStringAsync(cacheKey);
                if (savedOtp != otpCode)
                {
                    throw new InvalidOperationException("Failed to save OTP to cache.");
                }
            }
            catch (Exception ex)
            {
               
                throw new InvalidOperationException($"Failed to save OTP to Redis: {ex.Message}", ex);
            }

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
            catch (Exception ex)
            {
               
                throw new InvalidOperationException($"OTP saved but failed to send email: {ex.Message}", ex);
            }
        }

        public async Task<bool> ResetPassword(ResetPasswordWithOtpRequest request)
        {
            var user = await _unitOfWork.UserRepository.GetFirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null) return false;

            var cacheKey = $"otp:{request.Email}";
            var savedOtp = await _cache.GetStringAsync(cacheKey);

            var trimmedSavedOtp = savedOtp?.Trim();
            var trimmedRequestOtp = request.Otp?.Trim();

            if (string.IsNullOrEmpty(trimmedSavedOtp) || trimmedSavedOtp != trimmedRequestOtp)
            {
                return false;
            }

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
