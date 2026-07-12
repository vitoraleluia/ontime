using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnTime.Application.Services;
using OnTime.Site.Services;

namespace OnTime.Site.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IServerSideRenderer, ServerSideRenderer>();
        services.AddScoped<ICallbackUrlGenerator, CallbackUrlGenerator>();
        services.AddHttpContextAccessor();
        services.AddControllersWithViews();

        return services;
    }
}
