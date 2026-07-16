using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using OnTime.Application.Domain.Settings;
using OnTime.Application.Features.Images.Messages;
using OnTime.Application.Services;
using OnTime.Bus;
using OnTime.Infrastructure.Data;
using OnTime.Infrastructure.Services;

namespace OnTime.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ??
                               throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<IImageProcessor, ImageProcessor>();

        // Bus
        services.AddChannelBus<OptimizeImageMessage>();

        services.Configure<ImageStorageSettings>(configuration.GetSection(nameof(ImageStorageSettings)));
        services.Configure<ImageSizeSettings>(configuration.GetSection(nameof(ImageSizeSettings)));

        return services;
    }
}