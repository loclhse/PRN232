using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Application.Service;
using Application.DTOs.Request.Register;
using Application.DTOs.Request.Auth;
using Application.DTOs.Response.Auth;
using Microsoft.Extensions.Caching.Distributed;

namespace PRN2322.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous] 
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IDistributedCache _cache;

        public AuthController(IAuthService authService, IDistributedCache cache)
        {
            _authService = authService;
            _cache = cache;
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Credential))
            {
                return BadRequest("Credential is required.");
            }

            var result = await _authService.LoginWithGoogle(request.Credential);

            if (result == null)
            {
                return Unauthorized("Invalid Google credential.");
            }

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _authService.Login(request);

            if (result == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _authService.Register(request);

            if (!result)
            {
                return BadRequest("Username or Email already exists.");
            }

            return Ok(new { message = "Registration successful" });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenModel tokenModel)
        {
            if (tokenModel is null)
            {
                return BadRequest("Invalid client request");
            }

            var result = await _authService.RefreshToken(tokenModel);

            if (result == null)
            {
                return BadRequest("Invalid token or refresh token");
            }

            return Ok(result);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var result = await _authService.ForgotPassword(request.Email);
                return Ok(new { message = "If your email exists in our system, you will receive a reset link." });
            }
            catch (Exception ex)
            {
                // Log exception để debug (có thể thêm ILogger sau)
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordWithOtpRequest request)
        {
            if (!ModelState.IsValid) 
            {
                return BadRequest(new { 
                    message = "Validation failed", 
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            try
            {
                var result = await _authService.ResetPassword(request);

                if (!result)
                {
                    return BadRequest(new { 
                        message = "Invalid or expired OTP code.",
                        hint = "Please check your OTP code and ensure it hasn't expired (5 minutes)"
                    });
                }

                return Ok(new { message = "Password has been reset successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "An error occurred while resetting password.", 
                    error = ex.Message 
                });
            }
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email)) return Unauthorized();

            var result = await _authService.GetProfile(email);
            if (result == null) return NotFound("User not found.");

            return Ok(result);
        }

        // Debug endpoint - XÓA SAU KHI HOÀN THÀNH TEST
        [HttpGet("debug/check-otp/{email}")]
        public async Task<IActionResult> CheckOtp(string email)
        {
            var cacheKey = $"otp:{email}";
            var otp = await _cache.GetStringAsync(cacheKey);
            
            return Ok(new 
            { 
                Email = email,
                CacheKey = cacheKey,
                Otp = otp ?? "Not found or expired",
                Exists = otp != null,
                Message = otp != null ? "OTP found in Redis" : "OTP not found or expired (check within 5 minutes after forgot-password request)"
            });
        }
    }
}
