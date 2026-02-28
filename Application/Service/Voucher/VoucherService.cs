using AutoMapper;
using Domain.Entities;
using Domain.IUnitOfWork;
using Application.IService;
using Application.DTOs.Request.Voucher;
using Application.DTOs.Response.Voucher;

namespace Application.Service
{
    public class VoucherService : IVoucherService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public VoucherService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<VoucherResponse>> GetAllVouchersAsync()
        {
            var vouchers = await _unitOfWork.Repository<Voucher>().GetAllAsync();
            return _mapper.Map<IEnumerable<VoucherResponse>>(vouchers);
        }

        public async Task<VoucherResponse?> GetVoucherByIdAsync(Guid id)
        {
            var voucher = await _unitOfWork.Repository<Voucher>().GetByIdAsync(id);
            return voucher == null ? null : _mapper.Map<VoucherResponse>(voucher);
        }

        public async Task<VoucherResponse> CreateVoucherAsync(CreateVoucherRequest request)
        {
            // Kiểm tra trùng mã Code (Logic thêm để chặt chẽ)
            var existing = await _unitOfWork.Repository<Voucher>()
                .FindAsync(v => v.Code == request.Code);
            if (existing.Any())
            {
                throw new Exception($"Mã voucher '{request.Code}' đã tồn tại!");
            }

            var voucher = _mapper.Map<Voucher>(request);
            voucher.Id = Guid.NewGuid();

            await _unitOfWork.Repository<Voucher>().AddAsync(voucher);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<VoucherResponse>(voucher);
        }

        public async Task UpdateVoucherAsync(Guid id, UpdateVoucherRequest request)
        {
            var repo = _unitOfWork.Repository<Voucher>();
            var voucher = await repo.GetByIdAsync(id);

            if (voucher == null) throw new KeyNotFoundException("Voucher not found");

            _mapper.Map(request, voucher);

            repo.Update(voucher);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteVoucherAsync(Guid id)
        {
            var repo = _unitOfWork.Repository<Voucher>();
            var voucher = await repo.GetByIdAsync(id);

            if (voucher == null) throw new KeyNotFoundException("Voucher not found");

            repo.Remove(voucher);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}