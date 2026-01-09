using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Cấu hình Database thông qua Factory
            DbFactory.RegisterContext(services, configuration);

            // Đăng ký các Lớp (Repositories, UnitOfWork)
            // services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
