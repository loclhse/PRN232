using Application.IService;
using Application.Mappings;
using Application.Service;
using Application.Service.Category;
using Application.Service.Image;
using Application.Service.Order;
using Application.Service.Product;
using AutoMapper;
using Domain.IRepositories;
using Domain.IUnitOfWork;
using Infrastructure.Core;
using Infrastructure.Data;
using Infrastructure.Mappings;
using Infrastructure.Repositories;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Application.Service.Category;
using Application.Service.Product;
using Application.Service.Image;
using Application.Service.User;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Cấu hình Database thông qua Factory
            DbFactory.RegisterContext(services, configuration);

            services.AddHttpClient();

            // Cấu hình Redis Cache
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("Redis");
                options.InstanceName = "HappyBox_";
            });

            // Đăng ký các Lớp (Repositories, UnitOfWork)
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
            
       
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IMailService, MailService>();

            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IVoucherService, VoucherService>();
            services.AddScoped<IUserService, UserService>();

            // AutoMapper
            services.AddAutoMapper(cfg => 
            {
                cfg.AddMaps(typeof(MappingProfile).Assembly);
                cfg.AddMaps(typeof(InfrastructureProfile).Assembly);
            });

            // Cấu hình JWT Authentication
            var jwtSettings = configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"] ?? "SecretKeyMustBeAtLeast32CharactersLong");

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            return services;
        }
    }
}
