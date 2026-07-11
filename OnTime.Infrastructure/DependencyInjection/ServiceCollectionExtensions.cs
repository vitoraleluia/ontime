using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using OnTime.Domain.Entities;
using OnTime.Infrastructure.Services;
using OnTime.Infrastructure.Services.Emails;

namespace OnTime.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddTransient<IEmailSender<ApplicationUser>, EmailSender>();
        return services;
    }
}