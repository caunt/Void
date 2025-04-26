using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Services;

namespace Void.Proxy.Api.Extensions;

public static class HostingExtensions
{
    public static IServiceCollection AddJsonOptions(this IServiceCollection services)
    {
        return services.Configure<JsonSerializerOptions>(options =>
        {
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.WriteIndented = true;
            options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });
    }

    public static IServiceCollection AddSingletonAndListen<TService, TImplementation>(this IServiceCollection services) where TImplementation : class, TService where TService : class
    {
        services.AddSingleton<TService>(provider =>
        {
            var instance = ActivatorUtilities.GetServiceOrCreateInstance<TImplementation>(provider);

            if (instance is IEventListener listener)
            {
                var events = provider.GetRequiredService<IEventService>();
                events.RegisterListeners(listener);
            }

            return instance;
        });

        return services;
    }
}
