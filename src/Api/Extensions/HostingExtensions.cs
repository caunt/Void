using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Services;

namespace Void.Proxy.Api.Extensions;

public static class HostingExtensions
{
    private static readonly Type _constantCallSiteType;
    private static readonly Type _serviceIdentifierType;
    private static readonly ConstructorInfo _constantCallSiteConstructor;
    private static readonly MethodInfo _fromServiceTypeMethod;

    static HostingExtensions()
    {
        _constantCallSiteType = typeof(ServiceProvider).Assembly.GetType("Microsoft.Extensions.DependencyInjection.ServiceLookup.ConstantCallSite") ??
            throw new InvalidOperationException("Unable to find ConstantCallSite type in Microsoft.Extensions.DependencyInjection assembly. This may be due to a version mismatch or an internal change in the library.");

        _serviceIdentifierType = typeof(ServiceProvider).Assembly.GetType("Microsoft.Extensions.DependencyInjection.ServiceLookup.ServiceIdentifier") ??
            throw new InvalidOperationException("Unable to find ServiceIdentifier type in Microsoft.Extensions.DependencyInjection assembly. This may be due to a version mismatch or an internal change in the library.");

        _constantCallSiteConstructor = _constantCallSiteType.GetConstructor([typeof(Type), typeof(object)]) ??
            throw new InvalidOperationException("Unable to find ConstantCallSite constructor in Microsoft.Extensions.DependencyInjection assembly. This may be due to a version mismatch or an internal change in the library.");

        _fromServiceTypeMethod = _serviceIdentifierType.GetMethod("FromServiceType") ??
            throw new InvalidOperationException("Unable to find FromServiceType method in Microsoft.Extensions.DependencyInjection assembly. This may be due to a version mismatch or an internal change in the library.");
    }

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
        var hasDescriptor = services.Any(descriptor => descriptor.ServiceType == typeof(TInterface));
        return hasDescriptor;
    }

    public static bool HasService(this IServiceProvider serviceProvider, Type serviceType)
    {
        if (serviceProvider is not ServiceProvider)
        {
            serviceProvider = GetPropertyValue<ServiceProvider>(serviceProvider, "RootProvider") ??
                throw new InvalidOperationException("Unable to find RootProvider property in Microsoft.Extensions.DependencyInjection.ServiceProvider. This may be due to a version mismatch or an internal change in the library.");
        }

        var accessors = GetFieldValue<object>(serviceProvider, "_serviceAccessors");

        foreach (var pair in (IEnumerable)accessors!)
        {
            var key = GetFieldValue<object>(pair, "key") ??
                throw new InvalidOperationException("Unable to find key field in Microsoft.Extensions.DependencyInjection.ServiceProvider. This may be due to a version mismatch or an internal change in the library.");

            var identifierServiceType = GetPropertyValue<Type>(key, "ServiceType");

            if (identifierServiceType == serviceType)
                return true;
        }

        return serviceProvider.GetDescriptors().Any(descriptor => descriptor.ServiceType == serviceType);
    }

    public static void AddService(this IServiceProvider serviceProvider, ServiceDescriptor descriptor) // Type serviceType, object implementationInstance)
    {
        if (serviceProvider is not ServiceProvider)
        {
            serviceProvider = GetPropertyValue<ServiceProvider>(serviceProvider, "RootProvider") ??
                throw new InvalidOperationException("Unable to find RootProvider property in Microsoft.Extensions.DependencyInjection.ServiceProvider. This may be due to a version mismatch or an internal change in the library.");
        }

        var callSiteFactory = GetPropertyValue<object>(serviceProvider, "CallSiteFactory") ??
            throw new InvalidOperationException("Unable to find CallSiteFactory property in Microsoft.Extensions.DependencyInjection.ServiceProvider. This may be due to a version mismatch or an internal change in the library.");

        var descriptors = serviceProvider.GetDescriptors();
        serviceProvider.SetDescriptors([.. descriptors, descriptor]);

        CallMethod<object>(callSiteFactory, "Populate");

        // var serviceIdentifier = _fromServiceTypeMethod.Invoke(null, [serviceType]) ??
        //     throw new InvalidOperationException("Unable to find FromServiceType method in Microsoft.Extensions.DependencyInjection assembly. This may be due to a version mismatch or an internal change in the library.");
        // 
        // var callSite = _constantCallSiteConstructor.Invoke([serviceType, implementationInstance]);
        // 
        // CallMethod<object>(callSiteFactory, "Add", serviceIdentifier, callSite);

        static T? CallMethod<T>(object instance, string methodName, params object[] args)
        {
            return (T?)instance.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)!.Invoke(instance, args);
        }
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

    public static ServiceDescriptor[] GetDescriptors(this IServiceProvider provider)
    {
        var (instance, field) = provider.GetDescriptorsField();

        if (instance is null)
            return [];

        if (field.GetValue(instance) is not { } descriptorsValue)
            return [];

        return descriptorsValue as ServiceDescriptor[] ?? [];
    }

    public static void SetDescriptors(this IServiceProvider provider, ServiceDescriptor[] descriptors)
    {
        var (instance, field) = provider.GetDescriptorsField();

        if (instance is null)
            return;

        field.SetValue(instance, descriptors);
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

        foreach (var descriptor in provider.GetDescriptors())
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

    private static T? GetPropertyValue<T>(object instance, string propertyName)
    {
        var type = instance.GetType();
        var propertyInfo = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.GetProperty);
        var value = default(T);

        if (propertyInfo != null)
        {
            value = (T?)propertyInfo.GetValue(instance, null);
        }
        else
        {
            var baseType = type.BaseType;

            while (baseType != null)
            {
                propertyInfo = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.GetProperty);

                if (propertyInfo != null)
                {
                    value = (T?)propertyInfo.GetValue(instance, null);
                    break;
                }

                baseType = baseType.BaseType;
            }
        }

        return value;
    }

    private static T? GetFieldValue<T>(this object instance, string fieldName)
    {
        var type = instance.GetType();
        var fieldInfo = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.GetField);
        var value = default(T);

        if (fieldInfo != null)
        {
            value = (T?)fieldInfo.GetValue(instance);
        }
        else
        {
            var baseType = type.BaseType;

            while (baseType != null)
            {
                fieldInfo = baseType.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.GetField);

                if (fieldInfo != null)
                {
                    value = (T?)fieldInfo.GetValue(instance);
                    break;
                }

                baseType = baseType.BaseType;
            }
        }

        return value;
    }
}
