using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace OnTime.Api.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddControllers();

        services.AddAuthorization();

        services.AddEndpointsApiExplorer();

        services.AddSwaggerGenAuth(configuration);

        return services;
    }

    private static IServiceCollection AddSwaggerGenAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var authority = configuration["Authentication:Authority"];
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = authority;
                options.Audience = configuration["Authentication:Audience"];
                options.RequireHttpsMetadata = configuration.GetValue<bool>("Authentication:RequireHttpsMetadata");
                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = authority,
                };
            });


        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "OnTime API", Version = "v1" });

            options.AddSecurityDefinition(nameof(SecuritySchemeType.OAuth2),
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl =
                                new Uri($"{authority?.TrimEnd('/')}/protocol/openid-connect/auth"),
                            TokenUrl =
                                new Uri($"{authority?.TrimEnd('/')}/protocol/openid-connect/token"),
                            Scopes = new Dictionary<string, string>
                            {
                                { "openid", "OpenID Connect scope" }, { "profile", "User profile" }
                            }
                        }
                    }
                });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme, Id = nameof(SecuritySchemeType.OAuth2)
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }
}