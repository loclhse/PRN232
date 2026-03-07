using Application.DTOs.Request.Cart;
using Application.DTOs.Response.Cart;
using Application.DTOs.Response.Order;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.IUnitOfWork;

namespace Application.Service.Cart
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CartService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CartResponse?> GetCartByUserIdAsync(Guid userId)
        {
            var cart = await _unitOfWork.CartRepository.GetActiveCartByUserIdAsync(userId);
            if (cart == null)
                return null;

            return _mapper.Map<CartResponse>(cart);
        }

        public async Task<CartResponse> GetOrCreateCartAsync(Guid userId)
        {
            // Tìm cart hi?n có
            var cart = await _unitOfWork.CartRepository.GetActiveCartByUserIdAsync(userId);

            // N?u ch?a có thì t?o m?i
            if (cart == null)
            {
                cart = new Domain.Entities.Cart
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.CartRepository.AddAsync(cart);
                await _unitOfWork.SaveChangesAsync();

                // Reload ?? l?y ??y ?? thông tin
                cart = await _unitOfWork.CartRepository.GetCartWithItemsAsync(cart.Id);
            }

            return _mapper.Map<CartResponse>(cart);
        }

        public async Task<CartResponse> AddToCartAsync(Guid userId, AddToCartRequest request)
        {
            // Validate request
            if (!request.ProductId.HasValue && !request.GiftBoxId.HasValue)
            {
                throw new InvalidOperationException("Must provide either ProductId or GiftBoxId.");
            }

            // Lấy hoặc tạo cart
            var cart = await _unitOfWork.CartRepository.GetActiveCartByUserIdAsync(userId);
            if (cart == null)
            {
                cart = new Domain.Entities.Cart
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.CartRepository.AddAsync(cart);
                await _unitOfWork.SaveChangesAsync();
            }

            decimal unitPrice = 0;
            CartItem? existingItem = null;

            if (request.ProductId.HasValue)
            {
                // Validate Product
                var product = await _unitOfWork.ProductRepository.GetFirstOrDefaultAsync(
                    filter: p => p.Id == request.ProductId.Value && !p.IsDeleted && p.IsActive
                );

                if (product == null)
                {
                    throw new InvalidOperationException($"Product with ID '{request.ProductId}' not found or inactive.");
                }

                unitPrice = product.Price;

                // Check xem ?ã có trong gi? ch?a
                existingItem = await _unitOfWork.CartItemRepository.GetByCartAndProductAsync(cart.Id, request.ProductId.Value);
            }
            else if (request.GiftBoxId.HasValue)
            {
                // Validate GiftBox
                var giftBox = await _unitOfWork.GiftBoxRepository.GetFirstOrDefaultAsync(
                    filter: g => g.Id == request.GiftBoxId.Value && !g.IsDeleted && g.IsActive
                );

                if (giftBox == null)
                {
                    throw new InvalidOperationException($"GiftBox with ID '{request.GiftBoxId}' not found or inactive.");
                }

                unitPrice = giftBox.BasePrice;

                // Check xem ?ã có trong gi? ch?a
                existingItem = await _unitOfWork.CartItemRepository.GetByCartAndGiftBoxAsync(cart.Id, request.GiftBoxId.Value);
            }

            if (existingItem != null)
            {
                // ?ã có trong gi? -> C?p nh?t s? l??ng
                existingItem.Quantity += request.Quantity;
                existingItem.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.CartItemRepository.Update(existingItem);
            }
            else
            {
                // Ch?a có -> Thêm m?i
                var newItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = request.ProductId,
                    GiftBoxId = request.GiftBoxId,
                    Quantity = request.Quantity,
                    Price = unitPrice,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.CartItemRepository.AddAsync(newItem);
            }

            cart.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.CartRepository.Update(cart);
            await _unitOfWork.SaveChangesAsync();

            // Reload cart v?i ??y ?? thông tin
            var updatedCart = await _unitOfWork.CartRepository.GetCartWithItemsAsync(cart.Id);
            return _mapper.Map<CartResponse>(updatedCart);
        }

        public async Task<CartResponse?> UpdateCartItemAsync(Guid userId, Guid cartItemId, UpdateCartItemRequest request)
        {
            var cart = await _unitOfWork.CartRepository.GetActiveCartByUserIdAsync(userId);
            if (cart == null)
                return null;

            var cartItem = await _unitOfWork.CartItemRepository.GetFirstOrDefaultAsync(
                filter: ci => ci.Id == cartItemId && ci.CartId == cart.Id && !ci.IsDeleted
            );

            if (cartItem == null)
                return null;

            cartItem.Quantity = request.Quantity;
            cartItem.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.CartItemRepository.Update(cartItem);

            cart.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.CartRepository.Update(cart);

            await _unitOfWork.SaveChangesAsync();

            // Reload cart
            var updatedCart = await _unitOfWork.CartRepository.GetCartWithItemsAsync(cart.Id);
            return _mapper.Map<CartResponse>(updatedCart);
        }

        public async Task<bool> RemoveCartItemAsync(Guid userId, Guid cartItemId)
        {
            var cart = await _unitOfWork.CartRepository.GetActiveCartByUserIdAsync(userId);
            if (cart == null)
                return false;

            var cartItem = await _unitOfWork.CartItemRepository.GetFirstOrDefaultAsync(
                filter: ci => ci.Id == cartItemId && ci.CartId == cart.Id && !ci.IsDeleted
            );

            if (cartItem == null)
                return false;

            // Soft delete
            cartItem.IsDeleted = true;
            cartItem.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.CartItemRepository.Update(cartItem);

            cart.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.CartRepository.Update(cart);

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveCartItemsAsync(Guid userId, List<Guid> cartItemIds)
        {
            var cart = await _unitOfWork.CartRepository.GetActiveCartByUserIdAsync(userId);
            if (cart == null)
                return false;

            var cartItems = await _unitOfWork.CartItemRepository.FindAsync(
                filter: ci => cartItemIds.Contains(ci.Id) && ci.CartId == cart.Id && !ci.IsDeleted
            );

            if (!cartItems.Any())
                return false;

            foreach (var item in cartItems)
            {
                item.IsDeleted = true;
                item.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.CartItemRepository.Update(item);
            }

            cart.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.CartRepository.Update(cart);

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ClearCartAsync(Guid userId)
        {
            var cart = await _unitOfWork.CartRepository.GetActiveCartByUserIdAsync(userId);
            if (cart == null)
                return false;

            await _unitOfWork.CartItemRepository.ClearCartItemsAsync(cart.Id);

            cart.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.CartRepository.Update(cart);

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<OrderResponse> CheckoutAsync(Guid userId, CheckoutRequest request)
        {
            var cart = await _unitOfWork.CartRepository.GetCartWithItemsAsync(
                (await _unitOfWork.CartRepository.GetActiveCartByUserIdAsync(userId))?.Id ?? Guid.Empty
            );

            if (cart == null || !cart.Items.Any())
            {
                throw new InvalidOperationException("Cart is empty or not found.");
            }

            // L?c items c?n checkout
            var itemsToCheckout = request.SelectedItemIds != null && request.SelectedItemIds.Any()
                ? cart.Items.Where(i => request.SelectedItemIds.Contains(i.Id)).ToList()
                : cart.Items.ToList();

            if (!itemsToCheckout.Any())
            {
                throw new InvalidOperationException("No items selected for checkout.");
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Tính t?ng ti?n
                decimal totalAmount = 0;
                var orderDetails = new List<OrderDetail>();

                foreach (var item in itemsToCheckout)
                {
                    // L?y giá m?i nh?t t? DB (b?o m?t)
                    decimal currentPrice = 0;

                    if (item.ProductId.HasValue)
                    {
                        var product = await _unitOfWork.ProductRepository.GetByIdAsync(item.ProductId.Value);
                        if (product == null || product.IsDeleted || !product.IsActive)
                        {
                            throw new InvalidOperationException($"Product '{item.Product?.Name ?? item.ProductId.ToString()}' is no longer available.");
                        }
                        currentPrice = product.Price;

                        // Check inventory
                        var inventory = (await _unitOfWork.Repository<Inventory>().FindAsync(
                            filter: inv => inv.ProductId == item.ProductId && !inv.IsDeleted
                        )).FirstOrDefault();

                        if (inventory == null || inventory.Quantity < item.Quantity)
                        {
                            throw new InvalidOperationException($"Insufficient stock for product '{product.Name}'. Available: {inventory?.Quantity ?? 0}, Requested: {item.Quantity}");
                        }

                        // Tr? inventory
                        inventory.Quantity -= item.Quantity;
                        inventory.LastUpdated = DateTime.UtcNow;
                        if (inventory.Quantity == 0)
                            inventory.Status = InventoryStatus.OutOfStock;
                        else if (inventory.Quantity <= inventory.MinStockLevel)
                            inventory.Status = InventoryStatus.LowStock;

                        _unitOfWork.Repository<Inventory>().Update(inventory);
                    }
                    else if (item.GiftBoxId.HasValue)
                    {
                        var giftBox = await _unitOfWork.GiftBoxRepository.GetByIdAsync(item.GiftBoxId.Value);
                        if (giftBox == null || giftBox.IsDeleted || !giftBox.IsActive)
                        {
                            throw new InvalidOperationException($"GiftBox '{item.GiftBox?.Name ?? item.GiftBoxId.ToString()}' is no longer available.");
                        }
                        currentPrice = giftBox.BasePrice;
                    }

                    totalAmount += item.Quantity * currentPrice;

                    orderDetails.Add(new OrderDetail
                    {
                        ProductId = item.ProductId,
                        GiftBoxId = item.GiftBoxId,
                        Quantity = item.Quantity,
                        UnitPrice = currentPrice,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                // Tính phí ship
                decimal shippingFee = totalAmount >= 500000 ? 0 : 30000;
                decimal discountAmount = 0;
                Guid? voucherId = null;

                // X? lý Voucher
                if (!string.IsNullOrEmpty(request.VoucherCode))
                {
                    var voucher = await _unitOfWork.Repository<Voucher>().GetFirstOrDefaultAsync(
                        filter: v => v.Code == request.VoucherCode && !v.IsDeleted
                    );

                    if (voucher == null)
                        throw new InvalidOperationException("Voucher not found.");

                    if (!voucher.IsActive || voucher.EndDate < DateTime.UtcNow)
                        throw new InvalidOperationException("Voucher has expired or is inactive.");

                    if (totalAmount < voucher.MinOrderValue)
                        throw new InvalidOperationException($"Order total must be at least {voucher.MinOrderValue:N0}? to use this voucher.");

                    if (voucher.UsageLimit <= 0)
                        throw new InvalidOperationException("Voucher usage limit exceeded.");

                    // Tính discount
                    if (voucher.DiscountType == "PERCENT")
                    {
                        discountAmount = totalAmount * (voucher.Value / 100);
                        if (voucher.MaxDiscountAmount.HasValue)
                            discountAmount = Math.Min(discountAmount, voucher.MaxDiscountAmount.Value);
                    }
                    else
                    {
                        discountAmount = voucher.Value;
                    }

                    voucherId = voucher.Id;
                    voucher.UsageLimit -= 1;
                    _unitOfWork.Repository<Voucher>().Update(voucher);
                }

                // T?o Order
                var order = new Domain.Entities.Order
                {
                    OrderNumber = $"ORD-{DateTime.Now:yyyyMMddHHmmss}-{Guid.NewGuid().ToString()[..4].ToUpper()}",
                    UserId = userId,
                    CartId = cart.Id,
                    VoucherId = voucherId,
                    TotalAmount = totalAmount,
                    DiscountAmount = discountAmount,
                    ShippingFee = shippingFee,
                    ShippingAddress = request.ShippingAddress,
                    ShippingPhone = request.ShippingPhone,
                    FinalAmount = totalAmount - discountAmount + shippingFee,
                    CurrentStatus = OrderStatus.Pending,
                    Note = request.Note,
                    CreatedAt = DateTime.UtcNow,
                    OrderDetails = orderDetails
                };

                await _unitOfWork.OrderRepository.AddAsync(order);

                // T?o Order History
                var history = new OrderHistory
                {
                    OrderId = order.Id,
                    Status = OrderStatus.Pending,
                    Note = "Order created from cart checkout",
                    ChangedBy = "System",
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.Repository<OrderHistory>().AddAsync(history);

                // T?o Payment record
                var payment = new Payment
                {
                    OrderId = order.Id,
                    PaymentMethod = "COD", // Default COD, có th? m? r?ng sau
                    Status = "Pending",
                    Amount = order.FinalAmount,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.Repository<Payment>().AddAsync(payment);

                // Xóa các items ?ã checkout kh?i cart (soft delete)
                foreach (var item in itemsToCheckout)
                {
                    item.IsDeleted = true;
                    item.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.CartItemRepository.Update(item);
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                // Reload order v?i ??y ?? thông tin
                var createdOrder = await _unitOfWork.OrderRepository.GetFirstOrDefaultAsync(
                    filter: o => o.Id == order.Id,
                    includeProperties: "OrderDetails,OrderHistories,User"
                );

                return _mapper.Map<OrderResponse>(createdOrder);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<int> GetCartItemCountAsync(Guid userId)
        {
            var cart = await _unitOfWork.CartRepository.GetActiveCartByUserIdAsync(userId);
            if (cart == null)
                return 0;

            return cart.Items.Where(i => !i.IsDeleted).Sum(i => i.Quantity);
        }
    }
}
