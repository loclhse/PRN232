using AutoMapper;
using Domain.Entities;
using Domain.IUnitOfWork;
using Application.IService;
using Application.DTOs.Request;
using Application.DTOs.Response;

namespace Application.Service
{
    public class ImageService : IImageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ImageService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ImageResponse> CreateImageAsync(CreateImageRequest request)
        {
            // 1. Map từ Request sang Entity
            var image = _mapper.Map<Image>(request);
            image.Id = Guid.NewGuid(); // Tạo ID mới

            // 2. Dùng Generic Repository để thêm
            // Lưu ý: Repository<Image>() tự động hiểu là làm việc với bảng Image
            await _unitOfWork.Repository<Image>().AddAsync(image);
            await _unitOfWork.SaveChangesAsync();

            // 3. Trả về kết quả
            return _mapper.Map<ImageResponse>(image);
        }

        public async Task<IEnumerable<ImageResponse>> GetImagesByProductAsync(Guid productId)
        {
            // Use a method that exists in IGenericRepository, such as GetAllAsync or FindAsync
            var images = await _unitOfWork.Repository<Image>()
                                          .FindAsync(x => x.ProductId == productId);

            return _mapper.Map<IEnumerable<ImageResponse>>(images);
        }

        public async Task DeleteImageAsync(Guid id)
        {
            var repo = _unitOfWork.Repository<Image>();
            var image = await repo.GetByIdAsync(id);

            // THAY ĐỔI: Ném lỗi ngay nếu không tìm thấy
            if (image == null)
            {
                throw new KeyNotFoundException("Không tìm thấy ảnh với ID này trong Database!");
            }

            repo.Remove(image);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}