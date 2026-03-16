using Application.DTOs.Request.GiftBox;
using Application.DTOs.Response.GiftBox;
using Application.DTOs.Response;
using Application.Service.GiftBox;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PRN2322.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GiftBoxController : ControllerBase
    {
        private readonly IGiftBoxService _giftBoxService;

        public GiftBoxController(IGiftBoxService giftBoxService)
        {
            _giftBoxService = giftBoxService;
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim =
                User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ??
                User.FindFirst("sub")?.Value ??
                User.FindFirst("userId")?.Value ??
                User.FindFirst("UserId")?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim))
                throw new UnauthorizedAccessException("Không tìm thấy UserId trong JWT.");

            if (!Guid.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException("UserId trong JWT không hợp lệ.");

            return userId;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<GiftBoxResponse>>>> GetAllGiftBoxes()
        {
            try
            {
                var giftBoxes = await _giftBoxService.GetAllGiftBoxesAsync();
                return Ok(ApiResponse<IEnumerable<GiftBoxResponse>>.SuccessResponse(giftBoxes, "GiftBoxes retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<GiftBoxResponse>>.FailureResponse("An error occurred while retrieving gift boxes.", new List<string> { ex.Message }));
            }
        }

        // Lấy danh sách GiftBox đang active
        [HttpGet("active")]
        public async Task<ActionResult<ApiResponse<IEnumerable<GiftBoxResponse>>>> GetActiveGiftBoxes()
        {
            try
            {
                var giftBoxes = await _giftBoxService.GetActiveGiftBoxesAsync();
                return Ok(ApiResponse<IEnumerable<GiftBoxResponse>>.SuccessResponse(giftBoxes, "Active GiftBoxes retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<GiftBoxResponse>>.FailureResponse("An error occurred while retrieving active gift boxes.", new List<string> { ex.Message }));
            }
        }

        // Lấy danh sách GiftBox của User
        [Authorize]
        [HttpGet("user")]
        public async Task<ActionResult<ApiResponse<IEnumerable<GiftBoxResponse>>>> GetUserGiftBoxes()
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var giftBoxes = await _giftBoxService.GetGiftBoxesByUserIdAsync(currentUserId);
                return Ok(ApiResponse<IEnumerable<GiftBoxResponse>>.SuccessResponse(giftBoxes, "User GiftBoxes retrieved successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse<IEnumerable<GiftBoxResponse>>.FailureResponse(
                    "Bạn chưa đăng nhập hoặc token không hợp lệ.",
                    new List<string> { ex.Message }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<GiftBoxResponse>>.FailureResponse("An error occurred while retrieving user gift boxes.", new List<string> { ex.Message }));
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApiResponse<GiftBoxResponse>>> GetGiftBoxById(Guid id)
        {
            try
            {
                var giftBox = await _giftBoxService.GetGiftBoxByIdAsync(id);

                if (giftBox == null)
                {
                    return NotFound(ApiResponse<GiftBoxResponse>.FailureResponse("GiftBox not found."));
                }

                return Ok(ApiResponse<GiftBoxResponse>.SuccessResponse(giftBox, "GiftBox retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<GiftBoxResponse>.FailureResponse("An error occurred while retrieving gift box.", new List<string> { ex.Message }));
            }
        }

        // Lấy thông tin GiftBox theo Code
        [HttpGet("code/{code}")]
        public async Task<ActionResult<ApiResponse<GiftBoxResponse>>> GetGiftBoxByCode(string code)
        {
            try
            {
                var giftBox = await _giftBoxService.GetGiftBoxByCodeAsync(code);

                if (giftBox == null)
                {
                    return NotFound(ApiResponse<GiftBoxResponse>.FailureResponse("GiftBox not found."));
                }

                return Ok(ApiResponse<GiftBoxResponse>.SuccessResponse(giftBox, "GiftBox retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<GiftBoxResponse>.FailureResponse("An error occurred while retrieving gift box.", new List<string> { ex.Message }));
            }
        }

        // Lấy danh sách GiftBox theo Category
        [HttpGet("category/{categoryId:guid}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<GiftBoxResponse>>>> GetGiftBoxesByCategory(Guid categoryId)
        {
            try
            {
                var giftBoxes = await _giftBoxService.GetGiftBoxesByCategoryAsync(categoryId);
                return Ok(ApiResponse<IEnumerable<GiftBoxResponse>>.SuccessResponse(giftBoxes, "GiftBoxes retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<GiftBoxResponse>>.FailureResponse("An error occurred while retrieving gift boxes.", new List<string> { ex.Message }));
            }
        }

        [HttpPost]
        //[Authorize]
        public async Task<ActionResult<ApiResponse<GiftBoxResponse>>> CreateGiftBox([FromBody] CreateGiftBoxRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                return BadRequest(ApiResponse<GiftBoxResponse>.FailureResponse("Validation failed", errors));
            }

            try
            {
                var giftBox = await _giftBoxService.CreateGiftBoxAsync(request);
                return CreatedAtAction(
                    nameof(GetGiftBoxById),
                    new { id = giftBox.Id },
                    ApiResponse<GiftBoxResponse>.SuccessResponse(giftBox, "GiftBox created successfully")
                );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<GiftBoxResponse>.FailureResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<GiftBoxResponse>.FailureResponse("An error occurred while creating the gift box.", new List<string> { ex.Message }));
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ApiResponse<GiftBoxResponse>>> UpdateGiftBox(Guid id, [FromBody] UpdateGiftBoxRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                return BadRequest(ApiResponse<GiftBoxResponse>.FailureResponse("Validation failed", errors));
            }

            try
            {
                var giftBox = await _giftBoxService.UpdateGiftBoxAsync(id, request);

                if (giftBox == null)
                {
                    return NotFound(ApiResponse<GiftBoxResponse>.FailureResponse("GiftBox not found."));
                }

                return Ok(ApiResponse<GiftBoxResponse>.SuccessResponse(giftBox, "GiftBox updated successfully"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<GiftBoxResponse>.FailureResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<GiftBoxResponse>.FailureResponse("An error occurred while updating the gift box.", new List<string> { ex.Message }));
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ApiResponse>> DeleteGiftBox(Guid id)
        {
            try
            {
                var result = await _giftBoxService.DeleteGiftBoxAsync(id);

                if (!result)
                {
                    return NotFound(ApiResponse.FailureResponse("GiftBox not found."));
                }

                return Ok(ApiResponse.SuccessResponse("GiftBox deleted successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.FailureResponse("An error occurred while deleting gift box.", new List<string> { ex.Message }));
            }
        }
    }
}
