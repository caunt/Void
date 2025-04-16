using System.Diagnostics.CodeAnalysis;
using System.Reflection;
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
    private readonly Dictionary<IPlugin, IServiceProvider> _pluginServices = [];

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
        return (TService?)CreateInstance(serviceType) ??
            throw new InvalidOperationException($"Unable to cast instance of {serviceType} to {typeof(TService)}");
    }

    public object CreateInstance(Type serviceType)
    {
        var instance = ActivatorUtilities.CreateInstance(All, serviceType);

        if (serviceType.IsAssignableTo(typeof(IEventListener)))
            events.RegisterListeners((IEventListener)instance);

        if (serviceType.IsAssignableTo(typeof(IPlugin)))
            RegisterPlugin((IPlugin)instance);

        return instance;
    }

    public TService GetRequiredService<TService>() where TService : notnull
    {
        return Get(services => services.GetRequiredService<TService>());
    }

    public TService Get<TService>(Func<IServiceProvider, TService> provider)
    {
        return provider(All);
    }

    public void Register(Action<ServiceCollection> configure)
    {
        var services = new ServiceCollection();
        configure(services);

        if (!GetByAssembly(services[0].ServiceType.Assembly, out var plugin, out var existingServices))
            throw new InvalidOperationException("Source service provider is not found");

        existingServices.ForwardServices(services);
        _pluginServices[plugin] = services.BuildServiceProvider();
    }

    private ServiceProvider GetAll()
    {
        var forwardedServices = new ServiceCollection();
        services.ForwardServices(forwardedServices);

        foreach (var services in _pluginServices.Values)
            services.ForwardServices(forwardedServices);

        return forwardedServices.BuildServiceProvider();
    }

    private bool GetByAssembly(Assembly assembly, [MaybeNullWhen(false)] out IPlugin plugin, [MaybeNullWhen(false)] out IServiceProvider services)
    {
        foreach (var (_plugin, _services) in _pluginServices)
        {
            if (_plugin.GetType().Assembly != assembly)
                continue;

            plugin = _plugin;
            services = _services;
            return true;
        }

        plugin = null;
        services = null;
        return false;
    }

    private void RegisterPlugin(IPlugin plugin)
    {
        if (_pluginServices.ContainsKey(plugin))
            return;

        var services = new ServiceCollection();
        services.AddSingleton(plugin.GetType(), plugin);

        _pluginServices[plugin] = services.BuildServiceProvider();
    }
}
