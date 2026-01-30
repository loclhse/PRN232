using AutoMapper;
using Domain.Entities;
using Domain.IUnitOfWork;
using Application.DTOs.Request.Image;
using Application.DTOs.Response.Image;
namespace Application.Service.Image
{
    using ImageEntity = Domain.Entities.Image;

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
            var image = _mapper.Map<ImageEntity>(request);
            image.Id = Guid.NewGuid(); // Tạo ID mới

            // 2. Dùng Generic Repository để thêm
            // Lưu ý: Repository<ImageEntity>() tự động hiểu là làm việc với bảng Image
            await _unitOfWork.Repository<ImageEntity>().AddAsync(image);
            await _unitOfWork.SaveChangesAsync();

            // 3. Trả về kết quả
            return _mapper.Map<ImageResponse>(image);
        }

        public async Task<IEnumerable<ImageResponse>> GetImagesByProductAsync(Guid productId)
        {
            // Use a method that exists in IGenericRepository, such as GetAllAsync or FindAsync
            var images = await _unitOfWork.Repository<ImageEntity>()
                                          .FindAsync(x => x.ProductId == productId);

            return _mapper.Map<IEnumerable<ImageResponse>>(images);
        }

        public async Task DeleteImageAsync(Guid id)
        {
            var repo = _unitOfWork.Repository<ImageEntity>();
            var image = await repo.GetByIdAsync(id);

            // THAY ĐỔI: Ném lỗi ngay nếu không tìm thấy
            if (image == null)
            {
                throw new KeyNotFoundException("Không tìm thấy ảnh với ID này trong Database!");
            }

            repo.Remove(image);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<ImageResponse>> GetAllImagesAsync()
        {
            // Gọi Generic Repository để lấy tất cả
            var images = await _unitOfWork.Repository<Domain.Entities.Image>().GetAllAsync();

            // Map sang Response DTO
            return _mapper.Map<IEnumerable<ImageResponse>>(images);
        }

        public async Task UpdateImageAsync(Guid id, UpdateImageRequest request)
        {
            var repo = _unitOfWork.Repository<Domain.Entities.Image>();

            // 1. Tìm ảnh cũ trong DB
            var existingImage = await repo.GetByIdAsync(id);
            if (existingImage == null)
            {
                throw new KeyNotFoundException("Image not found");
            }

            // 2. Map dữ liệu mới đè lên dữ liệu cũ (AutoMapper xử lý việc này)
            _mapper.Map(request, existingImage);

            // Cập nhật thời gian update
            existingImage.UpdatedAt = DateTime.UtcNow; // Hoặc DateTime.Now tùy múi giờ server

            // 3. Gọi lệnh Update của Generic Repository
            repo.Update(existingImage);

            // 4. Lưu thay đổi
            await _unitOfWork.SaveChangesAsync();
        }
    }
}