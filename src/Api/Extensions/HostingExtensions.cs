using System.Text.Json;
using System.Text.Json.Serialization;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
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

    public static void Add(this IServiceProvider serviceProvider, ServiceDescriptor descriptor)
    {
        Add(serviceProvider.GetRequiredService<IContainer>(), descriptor);
    }

    public static void Add(this IContainer container, ServiceDescriptor descriptor)
    {
        // DryIocAdapter.cs copied here to inject Setup.With configuration
        // container.RegisterDescriptor(descriptor);

        var serviceKey = (object?)null;
        var ifAlreadyRegistered = IfAlreadyRegistered.Replace;

        var setup = Setup.With(weaklyReferenced: false, asResolutionCall: true);
        var serviceType = descriptor.ServiceType;
        var implementationType = descriptor.ImplementationType;

        if (implementationType is not null)
        {
            container.Register(
                ReflectionFactory.Of(
                    implementationType,
                    descriptor.Lifetime.ToReuse(),
                    setup: setup),
                serviceType,
                serviceKey,
                ifAlreadyRegistered,
                isStaticallyChecked: implementationType == serviceType);
        }
        else if (descriptor.ImplementationFactory is not null)
        {
            container.Register(
                DelegateFactory.Of(
                    descriptor.ImplementationFactory.ToFactoryDelegate,
                    descriptor.Lifetime.ToReuse(),
                    setup: setup),
                serviceType,
                serviceKey,
                ifAlreadyRegistered,
                isStaticallyChecked: true);
        }
        else
        {
            var instance = descriptor.ImplementationInstance;
            container.Register(
                InstanceFactory.Of(instance, setup: setup),
                serviceType,
                serviceKey,
                ifAlreadyRegistered,
                isStaticallyChecked: true);
            container.TrackDisposable(instance);
        }
    }

    public static void Remove(this IServiceProvider serviceProvider, Func<ServiceRegistrationInfo, bool> condition)
    {
        var container = serviceProvider.GetRequiredService<IContainer>();

        foreach (var service in container.GetServiceRegistrations())
        {
            if (!condition(service))
                continue;

            container.Unregister(service.ServiceType);
        }
    }
}
