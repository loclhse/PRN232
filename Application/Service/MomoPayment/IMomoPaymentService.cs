using Application.DTOs.Response.MomoPayment;
using Application.DTOs.Request.MomoPayment;

namespace Application.Service.MomoPayment
{
    public interface IMomoPaymentService
    {
        Task<MomoPaymentResponse> CreatePaymentAsync(Guid orderId, string? orderInfo = null);
        Task<MomoPaymentStatusResponse> QueryPaymentStatusAsync(Guid orderId);
        Task HandleIpnAsync(MomoIpnRequest request);
    }
}