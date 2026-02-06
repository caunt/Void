using System.Reflection;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Services;

namespace Void.Proxy.Plugins.Dependencies.Extensions;

public static class HostingExtensions
{
    /// <summary>
    /// Determines whether any registered service in the specified service collection has a constructor dependency on
    /// another registered service type. Does not consider factory or instance registrations.
    /// </summary>
    /// <remarks>This method examines the constructors of each registered implementation type in the service
    /// collection. It identifies relationships where a service's constructor depends on another registered service,
    /// including both concrete and open generic types. Factory and instance registrations are not inspected, as their
    /// dependencies cannot be determined statically.</remarks>
    /// <param name="serviceCollection">The collection of service descriptors to inspect for constructor-based relationships. This collection must not
    /// be null.</param>
    /// <returns>true if at least one registered service has a constructor parameter that matches another registered service
    /// type; otherwise, false.</returns>
    public static bool HasDescriptorRelationships(this IServiceCollection serviceCollection)
    {
        var registeredServiceTypes = serviceCollection.Select(serviceDescriptor => serviceDescriptor.ServiceType).ToHashSet();
        var openGenericServiceTypeDefinitions = registeredServiceTypes.Where(serviceType => serviceType.IsGenericTypeDefinition).ToHashSet();

        foreach (var serviceDescriptor in serviceCollection)
        {
            if (serviceDescriptor.ImplementationType is null)
                continue; // factory/instance: can't statically inspect

            var constructors = serviceDescriptor.ImplementationType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

            if (constructors.Length == 0)
                continue;

            var maxParameterCount = constructors.Max(constructorInfo => constructorInfo.GetParameters().Length);

            foreach (var constructorInfo in constructors.Where(constructorInfo => constructorInfo.GetParameters().Length == maxParameterCount))
            {
                foreach (var parameterInfo in constructorInfo.GetParameters())
                {
                    var parameterType = parameterInfo.ParameterType;

                    if (parameterType.IsGenericType && parameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                        parameterType = parameterType.GetGenericArguments()[0];

                    if (registeredServiceTypes.Contains(parameterType))
                        return true;

                    if (parameterType.IsGenericType && openGenericServiceTypeDefinitions.Contains(parameterType.GetGenericTypeDefinition()))
                        return true;
                }
            }
        }

        return false;
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
