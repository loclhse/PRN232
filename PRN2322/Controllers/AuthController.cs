using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Application.Service;
using Application.DTOs.Request.Register;
using Application.DTOs.Request.Auth;
using Application.DTOs.Response.Auth;
using Application.DTOs.Response;
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
        public async Task<ActionResult<ApiResponse<TokenModel>>> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Credential))
            {
                return BadRequest(ApiResponse<TokenModel>.FailureResponse("Credential is required."));
            }

            try
            {
                var result = await _authService.LoginWithGoogle(request.Credential);
                if (result == null)
                {
                    return Unauthorized(ApiResponse<TokenModel>.FailureResponse("Invalid Google credential or user creation failed."));
                }
                return Ok(ApiResponse<TokenModel>.SuccessResponse(result, "Login successful"));
            }
            catch (Exception ex)
            {
                return StatusCode(401, ApiResponse<TokenModel>.FailureResponse($"Google Login failed: {ex.Message}"));
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<TokenModel>>> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                return BadRequest(ApiResponse<TokenModel>.FailureResponse("Validation failed", errors));
            }

            var result = await _authService.Login(request);

            if (result == null)
            {
                return Unauthorized(ApiResponse<TokenModel>.FailureResponse("Invalid email or password."));
            }

            return Ok(ApiResponse<TokenModel>.SuccessResponse(result, "Login successful"));
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse>> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                return BadRequest(ApiResponse.FailureResponse("Validation failed", errors));
            }

            var result = await _authService.Register(request);

            if (!result)
            {
                return BadRequest(ApiResponse.FailureResponse("Username or Email already exists."));
            }

            return Ok(ApiResponse.SuccessResponse("Registration successful"));
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<ApiResponse<TokenModel>>> RefreshToken([FromBody] TokenModel tokenModel)
        {
            if (tokenModel is null)
            {
                return BadRequest(ApiResponse<TokenModel>.FailureResponse("Invalid client request"));
            }

            var result = await _authService.RefreshToken(tokenModel);

            if (result == null)
            {
                return BadRequest(ApiResponse<TokenModel>.FailureResponse("Invalid token or refresh token"));
            }

            return Ok(ApiResponse<TokenModel>.SuccessResponse(result, "Token refreshed successfully"));
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult<ApiResponse>> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                return BadRequest(ApiResponse.FailureResponse("Validation failed", errors));
            }

            try
            {
                var result = await _authService.ForgotPassword(request.Email);
                return Ok(ApiResponse.SuccessResponse("If your email exists in our system, you will receive a reset link."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.FailureResponse("An error occurred while processing your request.", new List<string> { ex.Message }));
            }
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult<ApiResponse>> ResetPassword([FromBody] ResetPasswordWithOtpRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                return BadRequest(ApiResponse.FailureResponse("Validation failed", errors));
            }

            try
            {
                var result = await _authService.ResetPassword(request);

                if (!result)
                {
                    return BadRequest(ApiResponse.FailureResponse("Invalid or expired OTP code."));
                }

                return Ok(ApiResponse.SuccessResponse("Password has been reset successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.FailureResponse("An error occurred while resetting password.", new List<string> { ex.Message }));
            }
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<ActionResult<ApiResponse<UserResponse>>> GetProfile()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized(ApiResponse<UserResponse>.FailureResponse("Unauthorized"));
            }

            var result = await _authService.GetProfile(email);
            if (result == null)
            {
                return NotFound(ApiResponse<UserResponse>.FailureResponse("User not found."));
            }

            return Ok(ApiResponse<UserResponse>.SuccessResponse(result, "Profile retrieved successfully"));
        }

      
    }
}
