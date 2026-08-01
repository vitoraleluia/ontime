using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;

using OnTime.Api.Domain.Settings;
using OnTime.Api.Extensions;

namespace OnTime.Api.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddControllers();
        services.AddAuthorization();

        services.Configure<AuthenticationSettings>(configuration.GetSection(nameof(AuthenticationSettings)));

        var authenticationSettings = configuration.GetRequiredSection(nameof(AuthenticationSettings))
            .Get<AuthenticationSettings>();
        if (!string.IsNullOrEmpty(authenticationSettings?.Google.ClientId) &&
            !string.IsNullOrEmpty(authenticationSettings?.Google.ClientSecret))
        {
            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = authenticationSettings.Google.ClientId;
                    options.ClientSecret = authenticationSettings.Google.ClientSecret;
                    options.SignInScheme = IdentityConstants.ExternalScheme;
                });
        }

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "OnTime API", Version = "v1" });
        });


        StringExtensions.Configure(authenticationSettings?.ClientUrl);

        return services;
    }
}