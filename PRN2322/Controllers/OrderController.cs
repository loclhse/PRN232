using Application.DTOs.Request.Order;
using Application.DTOs.Response.Order;
using Application.DTOs.Response;
using Application.Service.Order;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace PRN2322.Controllers
{
    [Route("api/orders")] // Dùng số nhiều theo chuẩn REST
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<OrderResponse>>>> GetAll()
        {
            var result = await _orderService.GetAllOrdersAsync();
            return Ok(ApiResponse<IEnumerable<OrderResponse>>.SuccessResponse(result, "Tải danh sách đơn hàng thành công."));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<OrderResponse>>> GetById(Guid id)
        {
            try
            {
                var result = await _orderService.GetOrderByIdAsync(id);

                if (result == null)
                {
                    return NotFound(ApiResponse<OrderResponse>.FailureResponse("Không tìm thấy đơn hàng."));
                }

                return Ok(ApiResponse<OrderResponse>.SuccessResponse(result, "Lấy thông tin đơn hàng thành công."));
            }
            catch (Exception ex)
            {
                // Trả về lỗi 500 kèm thông báo lỗi cụ thể để dễ debug giống các controller khác trong dự án
                return StatusCode(500, ApiResponse<OrderResponse>.FailureResponse(
                    "Đã xảy ra lỗi khi lấy thông tin đơn hàng.",
                    new List<string> { ex.Message }));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<OrderResponse>>> Create([FromBody] CreateOrderRequest request)
        {
            var result = await _orderService.CreateOrderAsync(request);
            return Ok(ApiResponse<OrderResponse>.SuccessResponse(result, "Tạo đơn hàng thành công."));
        }

        [HttpPatch("{id}/status")] // Dùng Patch để cập nhật một phần dữ liệu
        public async Task<ActionResult<ApiResponse<OrderResponse>>> UpdateStatus(Guid id, [FromBody] OrderStatus status)
        {
            var result = await _orderService.UpdateOrderStatusAsync(id, status);
            if (result == null) return NotFound(ApiResponse<OrderResponse>.FailureResponse("Cập nhật thất bại."));
            return Ok(ApiResponse<OrderResponse>.SuccessResponse(result, "Cập nhật trạng thái thành công."));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse>> Delete(Guid id)
        {
            var success = await _orderService.DeleteOrderAsync(id);
            if (!success) return NotFound(ApiResponse.FailureResponse("Xóa đơn hàng thất bại."));
            return Ok(ApiResponse.SuccessResponse("Xóa đơn hàng thành công."));
        }
    }
}