using Hangfire;
using Hangfire.PostgreSql;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using OnTime.Application.Domain.Settings;
using OnTime.Application.Services;
using OnTime.Domain.Settings;
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

        // Email & SMTP
        services.Configure<EmailSettings>(configuration.GetSection(nameof(EmailSettings)));
        services.AddScoped<IEmailSender, SmtpEmailSender>();

        // Hangfire with PostgreSQL storage
        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(options => options.UseNpgsqlConnection(connectionString)));

        services.AddHangfireServer();

        services.AddScoped<IImageJobService, ImageJobService>();

        services.Configure<ImageStorageSettings>(configuration.GetSection(nameof(ImageStorageSettings)));
        services.Configure<ImageSizeSettings>(configuration.GetSection(nameof(ImageSizeSettings)));

        return services;
    }
}