using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using OnTime.Application.Domain.Settings;
using OnTime.Application.Services;
using OnTime.Infrastructure.BackgroundServices;
using OnTime.Infrastructure.Data;
using OnTime.Infrastructure.Services;

namespace OnTime.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ??
                               throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<OnTimeDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<OnTimeDbContext>());
        services.AddScoped<IFileService, FileService>();
        services.AddSingleton<IImageProcessingQueue, ImageProcessingQueue>();
        services.AddScoped<IImageProcessor, ImageProcessor>();
        services.AddHostedService<ImageProcessingWorker>();

        services.Configure<ImageStorageSettings>(configuration.GetSection(nameof(ImageStorageSettings)));
        services.Configure<ImageSizeSettings>(configuration.GetSection(nameof(ImageSizeSettings)));

        return services;
    }
}