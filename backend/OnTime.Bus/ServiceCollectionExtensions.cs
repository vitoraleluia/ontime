using System;
using System.Linq;
using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;

namespace OnTime.Bus;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddChannelBus<TMessage>(this IServiceCollection services) where TMessage : IBusMessage
    {
        // 1. Register the unbounded Channel for the message type
        services.AddSingleton(Channel.CreateUnbounded<TMessage>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        }));

        // 2. Register the generic producer
        services.AddSingleton<IBusProducer<TMessage>, ChannelBusProducer<TMessage>>();

        // 3. Scan assemblies for any class inheriting from BusConsumer<TMessage> and register it as HostedService
        var consumerType = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .FirstOrDefault(t => !t.IsAbstract && 
                                 t.BaseType != null && 
                                 t.BaseType.IsGenericType && 
                                 t.BaseType.GetGenericTypeDefinition() == typeof(BusConsumer<>) && 
                                 t.BaseType.GetGenericArguments()[0] == typeof(TMessage));

        if (consumerType != null)
        {
            services.AddSingleton(typeof(Microsoft.Extensions.Hosting.IHostedService), consumerType);
        }

        return services;
    }
}
