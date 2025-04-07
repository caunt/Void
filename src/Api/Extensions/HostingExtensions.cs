using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

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

    public static void ForwardServices(this IServiceProvider provider, IServiceCollection collection)
    {
        foreach (var descriptor in provider.GetAllServices())
        {
            var factory = new Func<IServiceProvider, object>(_ => provider.GetRequiredService(descriptor.ServiceType));

            switch (descriptor.Lifetime)
            {
                case ServiceLifetime.Singleton:
                    {
                        if (descriptor.ImplementationType is { ContainsGenericParameters: true } type)
                            collection.AddSingleton(descriptor.ServiceType, type);
                        else
                            collection.AddSingleton(descriptor.ServiceType, factory);
                        break;
                    }
                case ServiceLifetime.Scoped:
                    {
                        if (descriptor.ImplementationType is { ContainsGenericParameters: true } type)
                            collection.AddScoped(descriptor.ServiceType, type);
                        else
                            collection.AddScoped(descriptor.ServiceType, factory);
                        break;
                    }
                case ServiceLifetime.Transient:
                    {
                        if (descriptor.ImplementationType is { ContainsGenericParameters: true } type)
                            collection.AddTransient(descriptor.ServiceType, type);
                        else
                            collection.AddTransient(descriptor.ServiceType, factory);
                        break;
                    }
                default:
                    throw new NotSupportedException($"Unsupported service lifetime: {descriptor.Lifetime}");
            }
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
