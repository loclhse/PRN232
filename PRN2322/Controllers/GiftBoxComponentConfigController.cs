using Application.DTOs.Request.GiftBoxComponentConfig;
using Application.DTOs.Response;
using Application.DTOs.Response.GiftBoxComponentConfig;
using Application.Service.GiftBoxComponentConfig;
using Microsoft.AspNetCore.Mvc;

namespace PRN2322.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GiftBoxComponentConfigController : ControllerBase
    {
        private readonly IGiftBoxComponentConfigService _giftBoxComponentConfigService;

        public GiftBoxComponentConfigController(IGiftBoxComponentConfigService giftBoxComponentConfigService)
        {
            _giftBoxComponentConfigService = giftBoxComponentConfigService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<GiftBoxComponentConfigResponse>>>> GetAll()
        {
            try
            {
                var configs = await _giftBoxComponentConfigService.GetAllAsync();
                return Ok(ApiResponse<IEnumerable<GiftBoxComponentConfigResponse>>.SuccessResponse(configs, "GiftBoxComponentConfigs retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<GiftBoxComponentConfigResponse>>.FailureResponse("An error occurred while retrieving configs.", new List<string> { ex.Message }));
            }
        }

        // Lấy GiftBoxComponentConfig đang active
        [HttpGet("active")]
        public async Task<ActionResult<ApiResponse<IEnumerable<GiftBoxComponentConfigResponse>>>> GetActiveConfigs()
        {
            try
            {
                var configs = await _giftBoxComponentConfigService.GetActiveConfigsAsync();
                return Ok(ApiResponse<IEnumerable<GiftBoxComponentConfigResponse>>.SuccessResponse(configs, "Active GiftBoxComponentConfigs retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<GiftBoxComponentConfigResponse>>.FailureResponse("An error occurred while retrieving active configs.", new List<string> { ex.Message }));
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApiResponse<GiftBoxComponentConfigResponse>>> GetById(Guid id)
        {
            try
            {
                var config = await _giftBoxComponentConfigService.GetByIdAsync(id);

                if (config == null)
                {
                    return NotFound(ApiResponse<GiftBoxComponentConfigResponse>.FailureResponse("GiftBoxComponentConfig not found."));
                }

                return Ok(ApiResponse<GiftBoxComponentConfigResponse>.SuccessResponse(config, "GiftBoxComponentConfig retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<GiftBoxComponentConfigResponse>.FailureResponse("An error occurred while retrieving config.", new List<string> { ex.Message }));
            }
        }

        // Lấy GiftBoxComponentConfig theo Category (Wine, Chocolate, Flower, etc.)
        [HttpGet("category/{category}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<GiftBoxComponentConfigResponse>>>> GetByCategory(string category)
        {
            try
            {
                var configs = await _giftBoxComponentConfigService.GetByCategoryAsync(category);
                return Ok(ApiResponse<IEnumerable<GiftBoxComponentConfigResponse>>.SuccessResponse(configs, "GiftBoxComponentConfigs retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<GiftBoxComponentConfigResponse>>.FailureResponse("An error occurred while retrieving configs.", new List<string> { ex.Message }));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<GiftBoxComponentConfigResponse>>> Create([FromBody] CreateGiftBoxComponentConfigRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                return BadRequest(ApiResponse<GiftBoxComponentConfigResponse>.FailureResponse("Validation failed", errors));
            }

            try
            {
                var config = await _giftBoxComponentConfigService.CreateAsync(request);
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = config.Id },
                    ApiResponse<GiftBoxComponentConfigResponse>.SuccessResponse(config, "GiftBoxComponentConfig created successfully")
                );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<GiftBoxComponentConfigResponse>.FailureResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<GiftBoxComponentConfigResponse>.FailureResponse("An error occurred while creating config.", new List<string> { ex.Message }));
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ApiResponse<GiftBoxComponentConfigResponse>>> Update(Guid id, [FromBody] UpdateGiftBoxComponentConfigRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                return BadRequest(ApiResponse<GiftBoxComponentConfigResponse>.FailureResponse("Validation failed", errors));
            }

            try
            {
                var config = await _giftBoxComponentConfigService.UpdateAsync(id, request);

                if (config == null)
                {
                    return NotFound(ApiResponse<GiftBoxComponentConfigResponse>.FailureResponse("GiftBoxComponentConfig not found."));
                }

                return Ok(ApiResponse<GiftBoxComponentConfigResponse>.SuccessResponse(config, "GiftBoxComponentConfig updated successfully"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<GiftBoxComponentConfigResponse>.FailureResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<GiftBoxComponentConfigResponse>.FailureResponse("An error occurred while updating config.", new List<string> { ex.Message }));
            }
        }

        // Xóa GiftBoxComponentConfig (soft delete)
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ApiResponse>> Delete(Guid id)
        {
            try
            {
                var result = await _giftBoxComponentConfigService.DeleteAsync(id);

                if (!result)
                {
                    return NotFound(ApiResponse.FailureResponse("GiftBoxComponentConfig not found."));
                }

                return Ok(ApiResponse.SuccessResponse("GiftBoxComponentConfig deleted successfully"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse.FailureResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.FailureResponse("An error occurred while deleting config.", new List<string> { ex.Message }));
            }
        }
    }
}
