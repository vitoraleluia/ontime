using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using OnTime.Application.Domain;
using OnTime.Application.Services;
using OnTime.Identity.Domain.Settings;
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
        
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 5;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        // Configure Identity Options
        services.Configure<IdentityOptions>(options =>
        {
            // Password settings.
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 1;

            // Lockout settings.
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // User settings.
            options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            options.User.RequireUniqueEmail = false;
        });

        // Configure Application Cookie
        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.ExpireTimeSpan = TimeSpan.FromDays(7);

            options.LoginPath = "/Auth/Login";
            options.AccessDeniedPath = "/Auth/AccessDenied";
            options.SlidingExpiration = true;
        });

        // Add Authentication & Google External Authentication
        services.AddAuthentication()
            .AddGoogle(options =>
            {
                var authSettings = configuration.GetSection(nameof(AuthenticationSettings))
                    .Get<AuthenticationSettings>();
                var settings = authSettings?.Google;
                options.ClientId = !string.IsNullOrWhiteSpace(settings?.ClientId) ? settings.ClientId : "placeholder-id";
                options.ClientSecret = !string.IsNullOrWhiteSpace(settings?.ClientSecret)
                    ? settings.ClientSecret
                    : "placeholder-secret";
            });

        // Configure options
        services.Configure<AuthenticationSettings>(
            configuration.GetSection(nameof(AuthenticationSettings)));
        services.Configure<EmailSettings>(
            configuration.GetSection(nameof(EmailSettings)));

        services.AddScoped<IIdentityService, IdentityService>();
        services.AddTransient<IEmailSender<ApplicationUser>, EmailSender>();

        return services;
    }
}
