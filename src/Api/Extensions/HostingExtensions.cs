using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;
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

    public static bool HasService<TInterface>(this IServiceCollection services)
    {
        return services.Any(descriptor => descriptor.ServiceType == typeof(TInterface));
    }

    public static void RegisterListeners(this IServiceCollection services)
    {
        var provider = services.BuildServiceProvider();
        services.Clear();

        provider.ForwardServices(services, (provider, serviceType) =>
        {
            var logger = provider.GetRequiredService<ILogger<IProxy>>();
            var instance = provider.GetRequiredService(serviceType);

            if (instance is IEventListener listener)
            {
                var events = provider.GetRequiredService<IEventService>();
                events.RegisterListeners(listener);

                logger.LogTrace("Registered {Type} event listener", listener);
            }

            return instance;
        });
    }

    public static ServiceDescriptor[] GetAllServices(this IServiceProvider provider)
    {
        var (instance, field) = provider.GetDescriptorsField();

        if (instance is null)
            return [];

        if (field.GetValue(instance) is not { } descriptorsValue)
            return [];

        return descriptorsValue as ServiceDescriptor[] ?? [];
    }

    public static void RemoveServicesByAssembly(this IServiceProvider provider, Assembly assembly)
    {
        var (instance, field) = provider.GetDescriptorsField();

        if (instance is null)
            return;

        if (field.GetValue(instance) is not ServiceDescriptor[] { } descriptorsValue)
            return;

        field.SetValue(instance, descriptorsValue.Where(serviceDescriptor => serviceDescriptor.ServiceType.Assembly != assembly).ToArray());
    }

    public static void ForwardServices(this IServiceProvider provider, IServiceCollection collection, Func<IServiceProvider, Type, object>? customFactory = null)
    {
        customFactory ??= (provider, serviceType) => provider.GetRequiredService(serviceType);

        foreach (var descriptor in provider.GetAllServices())
        {
            if (descriptor.ImplementationType is { ContainsGenericParameters: true } type)
            {
                collection.Add(descriptor);
                continue;
            }

            if (descriptor.ServiceType.IsAssignableTo(typeof(IHostedService)))
            {
                // TODO: Support hosted services forwarding via custom factory
                collection.Add(descriptor);
                continue;
            }

            collection.Add(ServiceDescriptor.Describe(descriptor.ServiceType, _ => customFactory(provider, descriptor.ServiceType), descriptor.Lifetime));
        }
    }

    private static (object instance, FieldInfo) GetDescriptorsField(this IServiceProvider provider)
    {
        var root = provider;
        var rootType = root.GetType();

        if (rootType.Name is "ServiceProviderEngineScope")
        {
            if (rootType.GetProperty("RootProvider", BindingFlags.Instance | BindingFlags.NonPublic) is not { } rootProviderProperty)
                return default;

            if (rootProviderProperty.GetValue(provider) is not { } rootProviderValue)
                return default;

            root = (IServiceProvider)rootProviderValue;
        }

        if (typeof(ServiceProvider).GetProperty("CallSiteFactory", BindingFlags.Instance | BindingFlags.NonPublic) is not { } callSiteFactoryProperty)
            return default;

        if (callSiteFactoryProperty.GetValue(root) is not { } callSiteFactoryValue)
            return default;

        if (callSiteFactoryValue.GetType().GetField("_descriptors", BindingFlags.Instance | BindingFlags.NonPublic) is not { } descriptorsProperty)
            return default;

        return (callSiteFactoryValue, descriptorsProperty);
    }
}
