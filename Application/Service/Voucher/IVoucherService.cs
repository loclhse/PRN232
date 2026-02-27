using Application.DTOs.Request.Voucher;
using Application.DTOs.Response.Voucher;

namespace Application.IService
{
    public interface IVoucherService
    {
        Task<IEnumerable<VoucherResponse>> GetAllVouchersAsync();
        Task<VoucherResponse?> GetVoucherByIdAsync(Guid id);
        Task<VoucherResponse> CreateVoucherAsync(CreateVoucherRequest request);
        Task UpdateVoucherAsync(Guid id, UpdateVoucherRequest request);
        Task DeleteVoucherAsync(Guid id);
    }
}