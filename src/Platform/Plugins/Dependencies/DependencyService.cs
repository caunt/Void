using Void.Common.Events;
using Void.Common.Plugins;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Plugins;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Extensions;
using Void.Proxy.Api.Plugins;

namespace Void.Proxy.Plugins.Dependencies;

public class DependencyService(IServiceProvider services, IEventService events) : IDependencyService
{
    private readonly Dictionary<IPlugin, IServiceProvider> _pluginServices = new();

    public IServiceProvider All => GetAll();

    [Subscribe]
    public void OnPluginUnload(PluginUnloadEvent @event)
    {
        if (!_pluginServices.TryGetValue(@event.Plugin, out var serviceProvider))
            return;

        serviceProvider.RemoveServicesByAssembly(@event.Plugin.GetType().Assembly);
        _pluginServices.Remove(@event.Plugin);
    }

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

    public TService Get<TService>(Func<IServiceProvider, TService> configure)
    {
        return configure(All);
    }

    public void Register(Action<ServiceCollection> configure)
    {
        var services = new ServiceCollection();
        configure(services);
    }

    private ServiceProvider GetAll()
    {
        var forwardedServices = new ServiceCollection();

        foreach (var services in _pluginServices.Values)
            services.ForwardServices(forwardedServices);

        return forwardedServices.BuildServiceProvider();
    }
}
