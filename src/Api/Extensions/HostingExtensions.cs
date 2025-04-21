using System.Text.Json;
using System.Text.Json.Serialization;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

    private static bool IsSupported(this ServiceDescriptor descriptor)
    {
        // TODO: implement these services event listener registration

        if (descriptor.ServiceType.ContainsGenericParameters)
            return false;

        if (descriptor.ServiceType.IsAssignableTo(typeof(IHostedService)))
            return false;

        return true;
    }

    public static object? CreateInstance(this ServiceDescriptor descriptor, IServiceProvider provider)
    {
        // TODO: implement these services event listener registration
        if (!descriptor.IsSupported())
            return null;

        return descriptor switch
        {
            { ImplementationInstance: not null } => descriptor.ImplementationInstance,
            { ImplementationFactory: not null } => descriptor.ImplementationFactory(provider),
            { ImplementationType: not null } => ActivatorUtilities.CreateInstance(provider, descriptor.ImplementationType),
            _ => throw new InvalidOperationException($"Unable to create instance of {descriptor.ServiceType}.")
        };
    }

    public static void RegisterListeners(this IServiceCollection services)
    {
        for (int i = 0; i < services.Count; i++)
        {
            var descriptor = services[i];

            if (!descriptor.IsSupported())
                continue;

            services[i] = ServiceDescriptor.Describe(descriptor.ServiceType, provider =>
            {
                var instance = descriptor.CreateInstance(provider) ??
                    throw new InvalidOperationException($"Unable to create instance of {descriptor.ServiceType}.");

                if (instance is IEventListener listener)
                {
                    var events = provider.GetRequiredService<IEventService>();
                    events.RegisterListeners(listener);
                }

                return instance;
            }, descriptor.Lifetime);
        }
    }

    public static void Add(this IServiceProvider serviceProvider, ServiceDescriptor descriptor)
    {
        var container = serviceProvider.GetRequiredService<IContainer>();

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
