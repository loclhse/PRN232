using Application.DTOs.Request.Cart;
using Application.DTOs.Response;
using Application.DTOs.Response.Cart;
using Application.DTOs.Response.Order;
using Application.Service.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PRN2322.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // Lấy UserId từ JWT token
        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value
                ?? User.FindFirst("userId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("Invalid user token.");
            }

            return userId;
        }

        private static bool IsStockOrInventoryError(string message)
        {
            return message.Contains("Insufficient stock", StringComparison.OrdinalIgnoreCase)
                || message.Contains("inventory", StringComparison.OrdinalIgnoreCase);
        }

        // Lấy giỏ hàng của user hiện tại
        [HttpGet]
        public async Task<ActionResult<ApiResponse<CartResponse>>> GetCart()
        {
            try
            {
                var userId = GetCurrentUserId();
                var cart = await _cartService.GetOrCreateCartAsync(userId);
                return Ok(ApiResponse<CartResponse>.SuccessResponse(cart, "Cart retrieved successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse<CartResponse>.FailureResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<CartResponse>.FailureResponse("An error occurred while retrieving cart.", new List<string> { ex.Message }));
            }
        }

        // Lấy số lượng items trong giỏ (để hiển thị badge)
        [HttpGet("count")]
        public async Task<ActionResult<ApiResponse<int>>> GetCartItemCount()
        {
            try
            {
                var userId = GetCurrentUserId();
                var count = await _cartService.GetCartItemCountAsync(userId);
                return Ok(ApiResponse<int>.SuccessResponse(count, "Cart item count retrieved successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse<int>.FailureResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<int>.FailureResponse("An error occurred.", new List<string> { ex.Message }));
            }
        }

        // Thêm sản phẩm/GiftBox vào giỏ hàng
        [HttpPost("items")]
        public async Task<ActionResult<ApiResponse<CartResponse>>> AddToCart([FromBody] AddToCartRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                return BadRequest(ApiResponse<CartResponse>.FailureResponse("Validation failed", errors));
            }

            try
            {
                var userId = GetCurrentUserId();
                var cart = await _cartService.AddToCartAsync(userId, request);
                var successMessage = request.ProductId.HasValue
                    ? "Product added to cart successfully."
                    : "GiftBox added to cart successfully.";

                return Ok(ApiResponse<CartResponse>.SuccessResponse(cart, successMessage));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse<CartResponse>.FailureResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                if (IsStockOrInventoryError(ex.Message))
                {
                    return Conflict(ApiResponse<CartResponse>.FailureResponse(ex.Message));
                }

                return BadRequest(ApiResponse<CartResponse>.FailureResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<CartResponse>.FailureResponse("An error occurred while adding item to cart.", new List<string> { ex.Message }));
            }
        }

        // Cập nhật số lượng item trong giỏ
        [HttpPut("items/{cartItemId:guid}")]
        public async Task<ActionResult<ApiResponse<CartResponse>>> UpdateCartItem(Guid cartItemId, [FromBody] UpdateCartItemRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                return BadRequest(ApiResponse<CartResponse>.FailureResponse("Validation failed", errors));
            }

            try
            {
                var userId = GetCurrentUserId();
                var cart = await _cartService.UpdateCartItemAsync(userId, cartItemId, request);

                if (cart == null)
                {
                    return NotFound(ApiResponse<CartResponse>.FailureResponse("Cart item not found."));
                }

                return Ok(ApiResponse<CartResponse>.SuccessResponse(cart, "Cart item updated successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse<CartResponse>.FailureResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                if (IsStockOrInventoryError(ex.Message))
                {
                    return Conflict(ApiResponse<CartResponse>.FailureResponse(ex.Message));
                }

                return BadRequest(ApiResponse<CartResponse>.FailureResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<CartResponse>.FailureResponse("An error occurred while updating cart item.", new List<string> { ex.Message }));
            }
        }

        // Xóa một item khỏi giỏ hàng
        [HttpDelete("items/{cartItemId:guid}")]
        public async Task<ActionResult<ApiResponse>> RemoveCartItem(Guid cartItemId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _cartService.RemoveCartItemAsync(userId, cartItemId);

                if (!result)
                {
                    return NotFound(ApiResponse.FailureResponse("Cart item not found."));
                }

                return Ok(ApiResponse.SuccessResponse("Cart item removed successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse.FailureResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.FailureResponse("An error occurred while removing cart item.", new List<string> { ex.Message }));
            }
        }

        // Xóa nhiều items khỏi giỏ hàng
        [HttpDelete("items")]
        public async Task<ActionResult<ApiResponse>> RemoveCartItems([FromBody] List<Guid> cartItemIds)
        {
            if (cartItemIds == null || !cartItemIds.Any())
            {
                return BadRequest(ApiResponse.FailureResponse("No items specified for removal."));
            }

            try
            {
                var userId = GetCurrentUserId();
                var result = await _cartService.RemoveCartItemsAsync(userId, cartItemIds);

                if (!result)
                {
                    return NotFound(ApiResponse.FailureResponse("No cart items found."));
                }

                return Ok(ApiResponse.SuccessResponse("Cart items removed successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse.FailureResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.FailureResponse("An error occurred while removing cart items.", new List<string> { ex.Message }));
            }
        }

        // Xóa toàn bộ giỏ hàng
        [HttpDelete]
        public async Task<ActionResult<ApiResponse>> ClearCart()
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _cartService.ClearCartAsync(userId);

                if (!result)
                {
                    return NotFound(ApiResponse.FailureResponse("Cart not found."));
                }

                return Ok(ApiResponse.SuccessResponse("Cart cleared successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse.FailureResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.FailureResponse("An error occurred while clearing cart.", new List<string> { ex.Message }));
            }
        }

        // Checkout giỏ hàng - Tạo Order từ Cart
        [HttpPost("checkout")]
        public async Task<ActionResult<ApiResponse<OrderResponse>>> Checkout([FromBody] CheckoutRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                return BadRequest(ApiResponse<OrderResponse>.FailureResponse("Validation failed", errors));
            }

            try
            {
                var userId = GetCurrentUserId();
                var order = await _cartService.CheckoutAsync(userId, request);
                return Ok(ApiResponse<OrderResponse>.SuccessResponse(order, "Checkout successful. Order created."));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse<OrderResponse>.FailureResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                if (IsStockOrInventoryError(ex.Message))
                {
                    return Conflict(ApiResponse<OrderResponse>.FailureResponse(ex.Message));
                }

                return BadRequest(ApiResponse<OrderResponse>.FailureResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<OrderResponse>.FailureResponse("An error occurred during checkout.", new List<string> { ex.Message }));
            }
        }
    }
}
