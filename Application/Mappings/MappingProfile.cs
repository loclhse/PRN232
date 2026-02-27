using Application.DTOs.Request;
using Application.DTOs.Request.Category;
using Application.DTOs.Request.Image;
using Application.DTOs.Request.Inventory;
using Application.DTOs.Request.Order;
using Application.DTOs.Request.Product;
using Application.DTOs.Request.Register;
using Application.DTOs.Request.Voucher;
using Application.DTOs.Request.User;
using Application.DTOs.Response;
using Application.DTOs.Response.Auth;
using Application.DTOs.Response.Image;
using Application.DTOs.Response.Inventory;
using Application.DTOs.Response.Order;
using Application.DTOs.Response.Product;
using Application.DTOs.Response.Voucher;
using AutoMapper;
using Domain.Constants;
using Domain.Entities;
using Domain.Enums;

namespace Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User Mapping (DTO <-> Entity)
            CreateMap<RegisterRequest, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => RoleIds.Customer))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
            
            CreateMap<User, UserResponse>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.RoleName.ToString()));

            // Product Mapping (DTO <-> Entity)
            CreateMap<CreateProductRequest, Product>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.BoxComponents, opt => opt.Ignore())
                .ForMember(dest => dest.Inventories, opt => opt.Ignore())
                .ForMember(dest => dest.OrderDetails, opt => opt.Ignore())
                .ForMember(dest => dest.Images, opt => opt.Ignore());

            CreateMap<UpdateProductRequest, Product>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.BoxComponents, opt => opt.Ignore())
                .ForMember(dest => dest.Inventories, opt => opt.Ignore())
                .ForMember(dest => dest.OrderDetails, opt => opt.Ignore())
                .ForMember(dest => dest.Images, opt => opt.Ignore());

            CreateMap<Product, ProductResponse>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null));

            // Category Mapping (DTO <-> Entity)
            CreateMap<CreateCategoryRequest, Category>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.ParentCategory, opt => opt.Ignore())
                .ForMember(dest => dest.SubCategories, opt => opt.Ignore())
                .ForMember(dest => dest.Products, opt => opt.Ignore())
                .ForMember(dest => dest.GiftBoxes, opt => opt.Ignore());

            CreateMap<UpdateCategoryRequest, Category>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.ParentCategory, opt => opt.Ignore())
                .ForMember(dest => dest.SubCategories, opt => opt.Ignore())
                .ForMember(dest => dest.Products, opt => opt.Ignore())
                .ForMember(dest => dest.GiftBoxes, opt => opt.Ignore());

            CreateMap<Category, CategoryResponse>()
                .ForMember(dest => dest.ParentName, opt => opt.MapFrom(src => src.ParentCategory != null ? src.ParentCategory.Name : null));

            // Image Mapping (DTO <-> Entity)
            CreateMap<CreateImageRequest, Image>();
            CreateMap<Image, ImageResponse>();
            // Mapping cho Update Image
            CreateMap<UpdateImageRequest, Domain.Entities.Image>();

            // Inventory Mapping (DTO <-> Entity)
            CreateMap<CreateInventoryRequest, Domain.Entities.Inventory>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdated, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                .ForMember(dest => dest.Transactions, opt => opt.Ignore());

            CreateMap<UpdateInventoryRequest, Domain.Entities.Inventory>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.ProductId, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdated, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                .ForMember(dest => dest.Transactions, opt => opt.Ignore());

            CreateMap<Domain.Entities.Inventory, InventoryResponse>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null));

            // =====================================
            // Order Mapping
            // =====================================
            CreateMap<CreateOrderDetailRequest, OrderDetail>();

            CreateMap<OrderDetail, OrderDetailResponse>()
                // Backend tự tính TotalPrice cho từng món
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Quantity * src.UnitPrice));

            CreateMap<OrderHistory, OrderHistoryResponse>();

            CreateMap<Order, OrderResponse>()
                .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.OrderDetails))
                .ForMember(dest => dest.OrderHistories, opt => opt.MapFrom(src => src.OrderHistories));

            CreateMap<CreateOrderRequest, Order>()
                .ForMember(dest => dest.OrderNumber, opt => opt.MapFrom(src => "ORD-" + DateTime.Now.Ticks.ToString().Substring(10)))
                .ForMember(dest => dest.CurrentStatus, opt => opt.MapFrom(src => OrderStatus.Pending))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            // =====================================
            // Voucher Mapping
            // =====================================
            CreateMap<CreateVoucherRequest, Domain.Entities.Voucher>()
                 .ForMember(dest => dest.Id, opt => opt.Ignore())
                 .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                 .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                 // Đổi bool IsPercentage thành chuỗi lưu DB
                 .ForMember(dest => dest.DiscountType, opt => opt.MapFrom(src => src.IsPercentage ? "PERCENT" : "AMOUNT"));

            CreateMap<UpdateVoucherRequest, Domain.Entities.Voucher>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.DiscountType, opt => opt.MapFrom(src => src.IsPercentage ? "PERCENT" : "AMOUNT"));

            CreateMap<Domain.Entities.Voucher, VoucherResponse>()
                // Đổi chuỗi DB ngược lại thành bool cho FE
                .ForMember(dest => dest.IsPercentage, opt => opt.MapFrom(src => src.DiscountType == "PERCENT"));
        }
    }
}
