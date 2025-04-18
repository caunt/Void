using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections;
using System.Text.Json;
using System.Text.Json.Serialization;
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

        // What should I do with scoped services?
        if (descriptor.Lifetime is ServiceLifetime.Scoped)
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

    public static bool HasService<TService>(this IServiceCollection services)
    {
        return services.HasService(typeof(TService));
    }

    public static bool HasService(this IServiceCollection services, Type serviceType)
    {
        var hasDescriptor = services.Any(descriptor => descriptor.ServiceType == serviceType);
        return hasDescriptor;
    }

    public static bool HasService<TService>(this IServiceProvider serviceProvider)
    {
        return serviceProvider.HasService(typeof(TService));
    }

    public static bool HasService(this IServiceProvider serviceProvider, Type serviceType)
    {
        serviceProvider = serviceProvider.GetRootProvider();

        var accessors = serviceProvider.GetFieldValue<IEnumerable>("_serviceAccessors") ??
            throw new InvalidOperationException($"Unable to find _serviceAccessors field in {serviceProvider}. This may be due to a version mismatch or an internal change in the library.");

        foreach (var keyValuePair in accessors)
        {
            var key = keyValuePair.GetFieldValue("key") ??
                throw new InvalidOperationException($"Unable to find key field in {keyValuePair}. This may be due to a version mismatch or an internal change in the library.");

            var identifierServiceType = key.GetPropertyValue<Type>("ServiceType");

            if (identifierServiceType == serviceType)
                return true;
        }

        return serviceProvider.GetDescriptors().Any(descriptor => descriptor.ServiceType == serviceType);
    }

    public static void Add(this IServiceProvider serviceProvider, ServiceDescriptor descriptor)
    {
        serviceProvider = serviceProvider.GetRootProvider();

        var descriptors = serviceProvider.GetDescriptors();
        serviceProvider.SetDescriptors([.. descriptors, descriptor]);

        serviceProvider.GetCallSiteFactory().InvokeMethod("Populate");
    }

    public static void Remove(this IServiceProvider provider, Func<ServiceDescriptor, bool> condition)
    {
        var descriptors = provider.GetDescriptors();
        provider.SetDescriptors([.. descriptors.Where(condition)]);

        // TODO: may need to clear service provider cache?
    }

    public static ServiceDescriptor[] GetDescriptors(this IServiceProvider serviceProvider)
    {
        var descriptors = serviceProvider.GetCallSiteFactory().GetFieldValue<ServiceDescriptor[]>("_descriptors") ??
            throw new InvalidOperationException($"Unable to find _descriptors field in. This may be due to a version mismatch or an internal change in the library.");

        return descriptors;
    }

    public static void SetDescriptors(this IServiceProvider serviceProvider, ServiceDescriptor[] descriptors)
    {
        serviceProvider.GetCallSiteFactory().SetFieldValue("_descriptors", descriptors);
    }

    public static ServiceProvider GetRootProvider(this IServiceProvider serviceProvider)
    {
        return serviceProvider as ServiceProvider
            ?? serviceProvider.GetPropertyValue<ServiceProvider>("RootProvider")
            ?? throw new InvalidOperationException($"Unable to find RootProvider property in {serviceProvider}. This may be due to a version mismatch or an internal change in the library.");
    }

    public static object GetCallSiteFactory(this IServiceProvider serviceProvider)
    {
        serviceProvider = serviceProvider.GetRootProvider();

        var callSiteFactory = serviceProvider.GetPropertyValue("CallSiteFactory") ??
            throw new InvalidOperationException($"Unable to find CallSiteFactory property in {serviceProvider}. This may be due to a version mismatch or an internal change in the library.");

        return callSiteFactory;
    }
}
