using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.IService;
using Domain.Entities;
using Domain.IUnitOfWork;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using AutoMapper;

namespace Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly IMailService _mailService;
        private readonly IMapper _mapper;

        public AuthService(IUnitOfWork unitOfWork, ITokenService tokenService, IConfiguration configuration, IMailService mailService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _configuration = configuration;
            _mailService = mailService;
            _mapper = mapper;
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
                    user = _mapper.Map<User>(payload);
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

            var user = _mapper.Map<User>(request);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            await userRepository.AddAsync(user);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<bool> ForgotPassword(string email)
        {
            var userRepository = _unitOfWork.Repository<User>();
            var user = await userRepository.GetFirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return false;

            // Tạo mã OTP 6 chữ số
            var otpCode = new Random().Next(100000, 999999).ToString();
            user.OtpCode = otpCode;
            user.OtpExpiryTime = DateTime.UtcNow.AddMinutes(5); // OTP chỉ có hiệu lực trong 5 phút

            await _unitOfWork.SaveChangesAsync();

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
            var userRepository = _unitOfWork.Repository<User>();
            var user = await userRepository.GetFirstOrDefaultAsync(
                u => u.Email == request.Email && u.OtpCode == request.Otp && u.OtpExpiryTime > DateTime.UtcNow);

            if (user == null) return false;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.OtpCode = null;
            user.OtpExpiryTime = null;

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

        public async Task<UserResponse?> GetProfile(string email)
        {
            var userRepository = _unitOfWork.Repository<User>();
            var user = await userRepository.GetFirstOrDefaultAsync(
                u => u.Email == email,
                includeProperties: "Role");

            if (user == null) return null;

            return _mapper.Map<UserResponse>(user);
        }
    }
}
