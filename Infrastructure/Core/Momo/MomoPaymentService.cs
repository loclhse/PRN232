using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Application.DTOs.Request.MomoPayment;
using Application.DTOs.Response.MomoPayment;
using Application.Service.MomoPayment;
using Domain.Entities;
using Domain.Enums;
using Domain.IUnitOfWork;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Core.Momo
{
    public class MomoPaymentService : IMomoPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly MomoApiOptions _options;
        private readonly ILogger<MomoPaymentService> _logger;

        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true
        };

        public MomoPaymentService(
            IUnitOfWork unitOfWork,
            IHttpClientFactory httpClientFactory,
            IOptions<MomoApiOptions> options,
            ILogger<MomoPaymentService> logger)
        {
            _unitOfWork = unitOfWork;
            _httpClientFactory = httpClientFactory;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<MomoPaymentResponse> CreatePaymentAsync(Guid orderId, string? orderInfo = null)
        {
            ValidateConfig();

            var order = await _unitOfWork.OrderRepository.GetFirstOrDefaultAsync(
                o => o.Id == orderId && !o.IsDeleted,
                includeProperties: "Payments");

            if (order == null)
                throw new Exception("Không tìm thấy đơn hàng.");

            var payment = order.Payments
                .Where(x => !x.IsDeleted && x.PaymentMethod == "MOMO")
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefault();

            if (payment == null)
                throw new Exception("Đơn hàng này chưa có bản ghi Payment cho MOMO.");

            if (payment.Status == "Success")
                throw new Exception("Đơn hàng này đã thanh toán thành công rồi.");

            var amount = ToLongAmount(payment.Amount);
            if (amount < 1000)
                throw new Exception("MoMo yêu cầu số tiền thanh toán tối thiểu là 1000 VND.");

            // Dùng 1 mã ổn định cho cả orderId và requestId để retry idempotent dễ hơn
            if (string.IsNullOrWhiteSpace(payment.TransactionReference))
            {
                payment.TransactionReference = $"MOMO_{payment.Id:N}";
                payment.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Repository<Payment>().Update(payment);
            }

            var momoOrderId = payment.TransactionReference!;
            var requestId = momoOrderId;

            var finalOrderInfo = string.IsNullOrWhiteSpace(orderInfo)
                ? $"Thanh toán đơn hàng {order.OrderNumber}"
                : orderInfo;

            // Né ký tự & để chuỗi signature khỏi rối
            finalOrderInfo = finalOrderInfo.Replace("&", " ");

            var extraData = BuildExtraData(order.Id, order.OrderNumber);

            var rawSignature =
                $"accessKey={_options.AccessKey}&amount={amount}&extraData={extraData}" +
                $"&ipnUrl={_options.IpnUrl}&orderId={momoOrderId}&orderInfo={finalOrderInfo}" +
                $"&partnerCode={_options.PartnerCode}&redirectUrl={_options.RedirectUrl}" +
                $"&requestId={requestId}&requestType=captureWallet";

            var signature = ComputeHmacSha256(rawSignature, _options.SecretKey);

            var payload = new MomoCreateGatewayRequest
            {
                PartnerCode = _options.PartnerCode,
                RequestType = "captureWallet",
                IpnUrl = _options.IpnUrl,
                RedirectUrl = _options.RedirectUrl,
                OrderId = momoOrderId,
                Amount = amount,
                OrderInfo = finalOrderInfo,
                RequestId = requestId,
                ExtraData = extraData,
                Lang = "vi",
                Signature = signature
            };

            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(30);

            var httpResponse = await client.PostAsJsonAsync(_options.ApiUrl, payload);
            var rawResponse = await httpResponse.Content.ReadAsStringAsync();

            var momoResponse = JsonSerializer.Deserialize<MomoCreateGatewayResponse>(rawResponse, JsonOptions);

            if (!httpResponse.IsSuccessStatusCode || momoResponse == null)
            {
                payment.Status = "Failed";
                payment.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Repository<Payment>().Update(payment);

                await AddPaymentHistoryAsync(
                    payment.Id,
                    payment.Status,
                    rawResponse,
                    $"Gọi API create MoMo thất bại. HTTP {(int)httpResponse.StatusCode}");

                await _unitOfWork.SaveChangesAsync();

                throw new Exception($"MoMo trả lỗi HTTP {(int)httpResponse.StatusCode}: {rawResponse}");
            }

            if (momoResponse.ResultCode != 0)
            {
                payment.Status = "Failed";
                payment.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Repository<Payment>().Update(payment);

                await AddPaymentHistoryAsync(
                    payment.Id,
                    payment.Status,
                    rawResponse,
                    $"MoMo tạo link thất bại. ResultCode = {momoResponse.ResultCode}, Message = {momoResponse.Message}");

                await _unitOfWork.SaveChangesAsync();

                throw new Exception($"MoMo tạo link thất bại: {momoResponse.Message} ({momoResponse.ResultCode})");
            }

            payment.Status = "WaitingForPayment";
            payment.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Repository<Payment>().Update(payment);

            await AddPaymentHistoryAsync(
                payment.Id,
                payment.Status,
                rawResponse,
                "Tạo link thanh toán MoMo thành công. Đang chờ người dùng thanh toán.");

            await _unitOfWork.SaveChangesAsync();

            return new MomoPaymentResponse
            {
                OrderId = order.Id,
                MomoOrderId = momoOrderId,
                RequestId = requestId,
                Amount = payment.Amount,
                ResultCode = momoResponse.ResultCode,
                Message = momoResponse.Message ?? "Success",
                PayUrl = momoResponse.PayUrl,
                Deeplink = momoResponse.Deeplink,
                QrCodeUrl = momoResponse.QrCodeUrl,
                LocalPaymentStatus = payment.Status
            };
        }

        public async Task<MomoPaymentStatusResponse> QueryPaymentStatusAsync(Guid orderId)
        {
            ValidateConfig();

            var order = await _unitOfWork.OrderRepository.GetFirstOrDefaultAsync(
                o => o.Id == orderId && !o.IsDeleted,
                includeProperties: "Payments");

            if (order == null)
                throw new Exception("Không tìm thấy đơn hàng.");

            var payment = order.Payments
                .Where(x => !x.IsDeleted && x.PaymentMethod == "MOMO")
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefault();

            if (payment == null)
                throw new Exception("Đơn hàng này chưa có payment MOMO.");

            if (string.IsNullOrWhiteSpace(payment.TransactionReference))
                throw new Exception("Payment MOMO chưa có TransactionReference để query.");

            var requestId = Guid.NewGuid().ToString("N");
            var momoOrderId = payment.TransactionReference;

            var rawSignature =
                $"accessKey={_options.AccessKey}&orderId={momoOrderId}&partnerCode={_options.PartnerCode}" +
                $"&requestId={requestId}";

            var signature = ComputeHmacSha256(rawSignature, _options.SecretKey);

            var payload = new MomoQueryGatewayRequest
            {
                PartnerCode = _options.PartnerCode,
                RequestId = requestId,
                OrderId = momoOrderId,
                Lang = "vi",
                Signature = signature
            };

            var queryUrl = BuildQueryUrl(_options.ApiUrl);

            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(30);

            var httpResponse = await client.PostAsJsonAsync(queryUrl, payload);
            var rawResponse = await httpResponse.Content.ReadAsStringAsync();

            var momoResponse = JsonSerializer.Deserialize<MomoQueryGatewayResponse>(rawResponse, JsonOptions);

            if (!httpResponse.IsSuccessStatusCode || momoResponse == null)
            {
                throw new Exception($"MoMo query thất bại. HTTP {(int)httpResponse.StatusCode}: {rawResponse}");
            }

            // Chỉ auto-update local khi đã chắc chắn success.
            // Các trạng thái khác cứ giữ nguyên local payment status, để IPN là nguồn chính.
            if (momoResponse.ResultCode == 0 && payment.Status != "Success")
            {
                await MarkPaymentSuccessAsync(
                    order,
                    payment,
                    rawResponse,
                    momoResponse.TransId,
                    "Đồng bộ query MoMo báo thanh toán thành công.");
            }
            else
            {
                await AddPaymentHistoryAsync(
                    payment.Id,
                    payment.Status,
                    rawResponse,
                    $"Query trạng thái MoMo: {momoResponse.Message} ({momoResponse.ResultCode})");

                await _unitOfWork.SaveChangesAsync();
            }

            return new MomoPaymentStatusResponse
            {
                OrderId = order.Id,
                MomoOrderId = momoOrderId,
                RequestId = requestId,
                Amount = payment.Amount,
                TransId = momoResponse.TransId,
                ResultCode = momoResponse.ResultCode,
                Message = momoResponse.Message ?? string.Empty,
                LocalPaymentStatus = payment.Status
            };
        }

        public async Task HandleIpnAsync(MomoIpnRequest request)
        {
            try
            {
                ValidateConfig();

                if (!ValidateIpnSignature(request))
                {
                    _logger.LogWarning("IPN MoMo signature không hợp lệ cho OrderId: {OrderId}", request.OrderId);
                    return;
                }

                if (request.PartnerCode != _options.PartnerCode)
                {
                    _logger.LogWarning("IPN MoMo sai PartnerCode. Received: {PartnerCode}", request.PartnerCode);
                    return;
                }

                var payment = await _unitOfWork.Repository<Payment>().GetFirstOrDefaultAsync(
                    p => !p.IsDeleted
                         && p.PaymentMethod == "MOMO"
                         && p.TransactionReference == request.OrderId,
                    includeProperties: "Order");

                if (payment == null)
                {
                    _logger.LogWarning("Không tìm thấy Payment MOMO với TransactionReference = {OrderId}", request.OrderId);
                    return;
                }

                var expectedAmount = ToLongAmount(payment.Amount);
                if (expectedAmount != request.Amount)
                {
                    _logger.LogWarning(
                        "IPN amount mismatch. OrderId: {OrderId}, DB: {DbAmount}, IPN: {IpnAmount}",
                        request.OrderId, expectedAmount, request.Amount);
                    return;
                }

                var rawIpn = JsonSerializer.Serialize(request);

                if (request.ResultCode == 0)
                {
                    await MarkPaymentSuccessAsync(
                        payment.Order,
                        payment,
                        rawIpn,
                        request.TransId,
                        "MoMo IPN báo thanh toán thành công.");
                    return;
                }

                if (request.ResultCode == 9000)
                {
                    payment.Status = "WaitingForPayment";
                    payment.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.Repository<Payment>().Update(payment);

                    await AddPaymentHistoryAsync(
                        payment.Id,
                        payment.Status,
                        rawIpn,
                        $"MoMo IPN báo giao dịch được cấp quyền (9000). TransId = {request.TransId}");

                    await _unitOfWork.SaveChangesAsync();
                    return;
                }

                payment.Status = "Failed";
                payment.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Repository<Payment>().Update(payment);

                await AddPaymentHistoryAsync(
                    payment.Id,
                    payment.Status,
                    rawIpn,
                    $"MoMo IPN báo thanh toán thất bại. ResultCode = {request.ResultCode}, Message = {request.Message}, TransId = {request.TransId}");

                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý IPN MoMo");
            }
        }

        private async Task MarkPaymentSuccessAsync(
            Order order,
            Payment payment,
            string rawResponse,
            long? transId,
            string note)
        {
            payment.Status = "Success";
            payment.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Repository<Payment>().Update(payment);

            await AddPaymentHistoryAsync(
                payment.Id,
                payment.Status,
                rawResponse,
                $"{note} TransId = {transId}");

            if (order.CurrentStatus == OrderStatus.Pending)
            {
                order.CurrentStatus = OrderStatus.Confirmed;
                order.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.OrderRepository.Update(order);

                await AddOrderHistoryAsync(
                    order.Id,
                    OrderStatus.Confirmed,
                    "Hệ thống tự động xác nhận đơn hàng sau khi MoMo báo thanh toán thành công.");
            }

            await _unitOfWork.SaveChangesAsync();
        }

        private async Task AddPaymentHistoryAsync(Guid paymentId, string status, string? rawResponse, string note)
        {
            var history = new PaymentHistory
            {
                Id = Guid.NewGuid(),
                PaymentId = paymentId,
                Status = status,
                RawResponse = rawResponse,
                Note = note,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<PaymentHistory>().AddAsync(history);
        }

        private async Task AddOrderHistoryAsync(Guid orderId, OrderStatus status, string note)
        {
            var history = new OrderHistory
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                Status = status,
                Note = note,
                ChangedBy = "System",
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<OrderHistory>().AddAsync(history);
        }

        private bool ValidateIpnSignature(MomoIpnRequest request)
        {
            var rawSignature =
                $"accessKey={_options.AccessKey}&amount={request.Amount}&extraData={request.ExtraData}" +
                $"&message={request.Message}&orderId={request.OrderId}&orderInfo={request.OrderInfo}" +
                $"&orderType={request.OrderType}&partnerCode={request.PartnerCode}&payType={request.PayType}" +
                $"&requestId={request.RequestId}&responseTime={request.ResponseTime}" +
                $"&resultCode={request.ResultCode}&transId={request.TransId}";

            var computed = ComputeHmacSha256(rawSignature, _options.SecretKey);
            return string.Equals(computed, request.Signature, StringComparison.OrdinalIgnoreCase);
        }

        private void ValidateConfig()
        {
            if (string.IsNullOrWhiteSpace(_options.ApiUrl) ||
                string.IsNullOrWhiteSpace(_options.PartnerCode) ||
                string.IsNullOrWhiteSpace(_options.AccessKey) ||
                string.IsNullOrWhiteSpace(_options.SecretKey) ||
                string.IsNullOrWhiteSpace(_options.RedirectUrl) ||
                string.IsNullOrWhiteSpace(_options.IpnUrl))
            {
                throw new Exception("Cấu hình MomoAPI trong appsettings.json chưa đầy đủ.");
            }
        }

        private static long ToLongAmount(decimal amount)
        {
            return Convert.ToInt64(Math.Round(amount, 0, MidpointRounding.AwayFromZero));
        }

        private static string BuildExtraData(Guid orderId, string orderNumber)
        {
            var payload = new
            {
                orderId,
                orderNumber
            };

            var json = JsonSerializer.Serialize(payload);
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
        }

        private static string ComputeHmacSha256(string rawData, string secretKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var dataBytes = Encoding.UTF8.GetBytes(rawData);

            using var hmac = new HMACSHA256(keyBytes);
            var hashBytes = hmac.ComputeHash(dataBytes);

            return Convert.ToHexString(hashBytes).ToLowerInvariant();
        }

        private static string BuildQueryUrl(string createUrl)
        {
            if (createUrl.EndsWith("/create", StringComparison.OrdinalIgnoreCase))
            {
                return createUrl[..^"/create".Length] + "/query";
            }

            return createUrl.Replace("create", "query", StringComparison.OrdinalIgnoreCase);
        }

        // ===== Internal models để gọi MoMo =====

        private class MomoCreateGatewayRequest
        {
            public string PartnerCode { get; set; } = string.Empty;
            public string RequestType { get; set; } = string.Empty;
            public string IpnUrl { get; set; } = string.Empty;
            public string RedirectUrl { get; set; } = string.Empty;
            public string OrderId { get; set; } = string.Empty;
            public long Amount { get; set; }
            public string OrderInfo { get; set; } = string.Empty;
            public string RequestId { get; set; } = string.Empty;
            public string ExtraData { get; set; } = string.Empty;
            public string Lang { get; set; } = "vi";
            public string Signature { get; set; } = string.Empty;
        }

        private class MomoCreateGatewayResponse
        {
            public string? PartnerCode { get; set; }
            public string? OrderId { get; set; }
            public string? RequestId { get; set; }
            public long Amount { get; set; }
            public long ResponseTime { get; set; }
            public string? Message { get; set; }
            public int ResultCode { get; set; }
            public string? PayUrl { get; set; }
            public string? Deeplink { get; set; }
            public string? QrCodeUrl { get; set; }
        }

        private class MomoQueryGatewayRequest
        {
            public string PartnerCode { get; set; } = string.Empty;
            public string RequestId { get; set; } = string.Empty;
            public string OrderId { get; set; } = string.Empty;
            public string Lang { get; set; } = "vi";
            public string Signature { get; set; } = string.Empty;
        }

        private class MomoQueryGatewayResponse
        {
            public string? PartnerCode { get; set; }
            public string? RequestId { get; set; }
            public string? OrderId { get; set; }
            public string? ExtraData { get; set; }
            public long Amount { get; set; }
            public long? TransId { get; set; }
            public string? PayType { get; set; }
            public int ResultCode { get; set; }
            public string? Message { get; set; }
            public long ResponseTime { get; set; }
        }
    }
}