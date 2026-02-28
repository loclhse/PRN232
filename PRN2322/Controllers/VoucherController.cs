using Application.DTOs.Request.Voucher;
using Application.DTOs.Response;
using Application.DTOs.Response.Voucher;
using Application.IService;
using Microsoft.AspNetCore.Mvc;

namespace PRN2322.Controllers
{
    [Route("api/vouchers")]
    [ApiController]
    public class VoucherController : ControllerBase
    {
        private readonly IVoucherService _voucherService;

        public VoucherController(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<VoucherResponse>>>> GetAll()
        {
            var result = await _voucherService.GetAllVouchersAsync();
            return Ok(ApiResponse<IEnumerable<VoucherResponse>>.SuccessResponse(result, "Lấy danh sách Voucher thành công"));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<VoucherResponse>>> GetById(Guid id)
        {
            var result = await _voucherService.GetVoucherByIdAsync(id);
            if (result == null)
                return NotFound(ApiResponse<VoucherResponse>.FailureResponse("Không tìm thấy Voucher"));

            return Ok(ApiResponse<VoucherResponse>.SuccessResponse(result, "Tìm thấy Voucher"));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<VoucherResponse>>> Create([FromBody] CreateVoucherRequest request)
        {
            try
            {
                var result = await _voucherService.CreateVoucherAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<VoucherResponse>.SuccessResponse(result, "Tạo Voucher thành công"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.FailureResponse(ex.Message));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> Update(Guid id, [FromBody] UpdateVoucherRequest request)
        {
            try
            {
                await _voucherService.UpdateVoucherAsync(id, request);
                return Ok(ApiResponse<string>.SuccessResponse("Cập nhật thành công"));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<string>.FailureResponse("Voucher không tồn tại"));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(Guid id)
        {
            try
            {
                await _voucherService.DeleteVoucherAsync(id);
                return Ok(ApiResponse<string>.SuccessResponse("Xóa thành công"));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<string>.FailureResponse("Voucher không tồn tại"));
            }
        }
    }
}