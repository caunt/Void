using System.Collections;
using Void.Proxy.Api.Extensions;

namespace Void.Proxy.Extensions;

public static class HostingExtensions
{
    public static bool HasService(this IServiceProvider serviceProvider, Type serviceType)
    {
        serviceProvider = serviceProvider.GetRootProvider();

        var accessors = serviceProvider.GetFieldValue("_serviceAccessors") ??
            throw new InvalidOperationException($"Unable to find _serviceAccessors field in {serviceProvider}. This may be due to a version mismatch or an internal change in the library.");

        foreach (var keyValuePair in (IEnumerable)accessors)
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

        var callSiteFactory = serviceProvider.GetPropertyValue("CallSiteFactory") ??
            throw new InvalidOperationException($"Unable to find CallSiteFactory property in {serviceProvider}. This may be due to a version mismatch or an internal change in the library.");

        var descriptors = serviceProvider.GetDescriptors();
        serviceProvider.SetDescriptors([.. descriptors, descriptor]);

        callSiteFactory.InvokeMethod("Populate");
    }

    public static ServiceProvider GetRootProvider(this IServiceProvider serviceProvider)
    {
        return serviceProvider as ServiceProvider
            ?? serviceProvider.GetPropertyValue<ServiceProvider>("RootProvider")
            ?? throw new InvalidOperationException($"Unable to find RootProvider property in {serviceProvider}. This may be due to a version mismatch or an internal change in the library.");
    }
}
