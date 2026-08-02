using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using OnTime.Application.Services;
using OnTime.Domain.Settings;
using OnTime.Identity.Data;
using OnTime.Identity.Entities;
using OnTime.Identity.Services;

namespace OnTime.Identity.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ??
                               throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<AppIdentityDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddIdentityApiEndpoints<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AppIdentityDbContext>();

        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.Name = "OnTimeUserIdentity";
            options.Cookie.HttpOnly = true;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.ExpireTimeSpan = TimeSpan.FromDays(1);
            options.SlidingExpiration = true;
        });

        services.ConfigureExternalCookie(options =>
        {
            options.Cookie.Name = "OnTimeExternalIdentity";
            options.Cookie.HttpOnly = true;
            options.Cookie.SameSite = SameSiteMode.Lax;
        });

        var authenticationSettings = configuration.GetRequiredSection(nameof(AuthenticationSettings))
            .Get<AuthenticationSettings>();

        services.Configure<IdentityOptions>(options =>
        {
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(authenticationSettings?.Lockout.DefaultLockoutTimeSpanInMinutes ?? 5);
            options.Lockout.MaxFailedAccessAttempts = authenticationSettings?.Lockout.MaxFailedAccessAttempts ?? 5;
            options.Lockout.AllowedForNewUsers = true;
        });

        services.AddScoped<IIdentityService, IdentityService>();

        return services;
    }
}