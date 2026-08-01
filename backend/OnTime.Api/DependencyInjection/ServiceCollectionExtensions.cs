using Microsoft.OpenApi.Models;

using OnTime.Api.Domain.Settings;

namespace OnTime.Api.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddControllers();
        services.AddAuthorization();

        var authenticationSettings = configuration.GetRequiredSection("Authentication").Get<AuthenticationSettings>();
        if (!string.IsNullOrEmpty(authenticationSettings?.Google.ClientId) &&
            !string.IsNullOrEmpty(authenticationSettings?.Google.ClientSecret))
        {
            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = authenticationSettings.Google.ClientId;
                    options.ClientSecret = authenticationSettings.Google.ClientSecret;
                });
        }

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "OnTime API", Version = "v1" });
        });

        return services;
    }
}