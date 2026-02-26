using Infrastructure;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace PRN2322
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            
            // Web Services (CORS, Swagger, etc.)
            builder.Services.AddWebServices();

            // Infrastructure Services (DbContext, Repos, etc.)
            builder.Services.AddInfrastructure(builder.Configuration);

            var app = builder.Build();

            // Tự động chạy Migration khi khởi động app trên Render
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<AppDbContext>();
                    if (context.Database.GetPendingMigrations().Any())
                    {
                        context.Database.Migrate();
                    }
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while migrating the database.");
                }
            }

            // Configure the HTTP request pipeline.
            // Bật Swagger ở mọi môi trường để dễ dàng test trên Render
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            
            app.UseCors("AllowFrontend"); // Phải đặt trước Authentication và Authorization

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
