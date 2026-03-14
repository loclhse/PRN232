using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class TempFileCleanupService : BackgroundService
    {
        private readonly ILogger<TempFileCleanupService> _logger;
        private readonly string? _tempPath;

        public TempFileCleanupService(ILogger<TempFileCleanupService> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            
            if (!string.IsNullOrWhiteSpace(env.WebRootPath))
            {
                _tempPath = Path.Combine(env.WebRootPath, "images", "custom-baskets", "temp");
            }
            else
            {
                _logger.LogWarning("WebRootPath is null. TempFileCleanupService will not run.");
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (string.IsNullOrWhiteSpace(_tempPath))
            {
                _logger.LogWarning("TempFileCleanupService cannot start because temp path is not configured.");
                return;
            }

            _logger.LogInformation("TempFileCleanupService is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (Directory.Exists(_tempPath))
                    {
                        var files = Directory.GetFiles(_tempPath);
                        foreach (var file in files)
                        {
                            var fileInfo = new FileInfo(file);
                           
                            if (fileInfo.CreationTimeUtc < DateTime.UtcNow.AddHours(-24))
                            {
                                fileInfo.Delete();
                                _logger.LogInformation($"Deleted temp file: {fileInfo.Name}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error cleaning up temp files");
                }

              await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }

            _logger.LogInformation("TempFileCleanupService is stopping.");
        }
    }
}
