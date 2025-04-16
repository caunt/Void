using Void.Common.Events;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Plugins;

namespace Void.Proxy.Plugins.Dependencies;

public class DependencyService(IServiceProvider services, IEventService events) : IDependencyService
{
    public TService CreateInstance<TService>()
    {
        return CreateInstance<TService>(typeof(TService));
    }

    public TService CreateInstance<TService>(Type serviceType)
    {
        var service = (TService?)CreateInstance(serviceType) ?? throw new InvalidOperationException($"Unable to cast instance of {serviceType} to {typeof(TService)}");

        if (service.GetType().IsAssignableTo(typeof(IEventListener)))
            events.RegisterListeners((IEventListener)service);

        return service;
    }

    public object CreateInstance(Type serviceType)
    {
        return ActivatorUtilities.CreateInstance(services, serviceType);
    }

    public TService GetRequiredService<TService>() where TService : notnull
    {
        return services.GetRequiredService<TService>();
    }
}
