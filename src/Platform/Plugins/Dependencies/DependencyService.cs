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

public class DependencyService(ILogger<DependencyService> logger, IServiceProvider services, IEventService events) : IDependencyService
{
    private readonly Dictionary<IPlugin, IServiceProvider> _pluginServices = [];

    public IServiceProvider Services => GetAll();

    [Subscribe(PostOrder.First)]
    public void OnPluginUnload(PluginUnloadEvent @event)
    {
        if (!_pluginServices.TryGetValue(@event.Plugin, out _))
            return;

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
        var instance = Services.GetService(serviceType) ?? ActivatorUtilities.CreateInstance(Services, serviceType);

        if (serviceType.IsAssignableTo(typeof(IEventListener)))
            events.RegisterListeners((IEventListener)instance);

        if (serviceType.IsAssignableTo(typeof(IPlugin)))
            RegisterPlugin((IPlugin)instance);

        return instance;
    }

    public TService? GetService<TService>()
    {
        return Get(services => services.GetService<TService>());
    }

    public TService GetRequiredService<TService>() where TService : notnull
    {
        return Get(services => services.GetRequiredService<TService>());
    }

    public TService Get<TService>(Func<IServiceProvider, TService> provider)
    {
        return provider(Services);
    }

    public void Register(Action<ServiceCollection> configure)
    {
        var pluginServices = new ServiceCollection();
        configure(pluginServices);

        if (!GetByAssembly(pluginServices[0].ServiceType.Assembly, out var plugin, out var existingServices))
            throw new InvalidOperationException("Source service provider is not found");

        existingServices.ForwardServices(pluginServices);
        _pluginServices[plugin] = pluginServices.BuildServiceProvider();
    }

    private ServiceProvider GetAll()
    {
        var forwardedServices = new ServiceCollection();

        foreach (var services in _pluginServices.Values)
            services.ForwardServices(forwardedServices);

        if (_pluginServices.Values.Count is 0)
            services.ForwardServices(forwardedServices);

        return forwardedServices.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
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

        var pluginServices = new ServiceCollection();

        logger.LogTrace("Injecting {Plugin} into {Name} service collection", plugin.GetType(), plugin.Name);

        // Plugin => Plugin
        pluginServices.AddSingleton(plugin.GetType(), plugin);
        services.ForwardServices(pluginServices);

        _pluginServices[plugin] = pluginServices.BuildServiceProvider();
    }
}
