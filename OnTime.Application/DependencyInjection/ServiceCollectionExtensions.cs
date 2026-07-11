using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace OnTime.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(options =>
            options.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly())
        );

        return services;
    }
}