using System.Security.Claims;
using Application.DTOs.Request.Chatbot;
using Application.DTOs.Response;
using Application.Service.Chatbot;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PRN2322.Controllers
{
    [Route("api/custom-baskets")]
    [ApiController]
    [Authorize]
    public class CustomBasketController : ControllerBase
    {
        private readonly ICustomBasketImageService _customBasketImageService;

        public CustomBasketController(ICustomBasketImageService customBasketImageService)
        {
            _customBasketImageService = customBasketImageService;
        }



        [HttpPost("generate-image")]
        public async Task<ActionResult<ApiResponse<string>>> GenerateImage([FromBody] CreateCustomBasketRequest request)
        {

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(ApiResponse<string>.FailureResponse("Không xác định được người dùng từ token."));
            }

            try
            {
                var imageUrl = await _customBasketImageService.GenerateCustomBasketAsync(request, userId);
                return Ok(ApiResponse<string>.SuccessResponse(imageUrl, "Tạo ảnh giỏ quà tùy chỉnh thành công."));
            }

            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<string>.FailureResponse(ex.Message));
            }

            catch (Exception ex)
            {
                return StatusCode(500,
                    ApiResponse<string>.FailureResponse("Lỗi hệ thống khi tạo ảnh giỏ quà tùy chỉnh.",
                        new List<string> { ex.Message }));
            }
        }

        [HttpPost("confirm")]
        public async Task<ActionResult<ApiResponse<Guid>>> ConfirmCustomBasket([FromBody] ConfirmCustomBasketRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(ApiResponse<Guid>.FailureResponse("Không xác định được người dùng từ token."));
            }

            try
            {
                var giftBoxId = await _customBasketImageService.ConfirmCustomBasketAsync(request, userId);
                return Ok(ApiResponse<Guid>.SuccessResponse(giftBoxId, "Xác nhận giỏ quà tùy chỉnh thành công."));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<Guid>.FailureResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    ApiResponse<Guid>.FailureResponse("Lỗi hệ thống khi xác nhận giỏ quà tùy chỉnh.",
                        new List<string> { ex.Message }));
            }
        }
    }
}

