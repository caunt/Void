using System.Reflection;
using DryIoc;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Plugins;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Extensions;
using Void.Proxy.Api.Plugins;
using Void.Proxy.Api.Plugins.Dependencies;

namespace Void.Proxy.Plugins.Dependencies;

public class DependencyService(ILogger<DependencyService> logger, IServiceProvider serviceProvider, IEventService events) : IDependencyService
{
    private readonly Dictionary<Assembly, IContainer> _containers = [];

    [Subscribe(PostOrder.First)]
    public void OnPluginUnloading(PluginUnloadingEvent @event)
    {
        var assembly = @event.Plugin.GetType().Assembly;

        serviceProvider.Remove(descriptor => descriptor.ServiceType.Assembly == assembly);
        events.UnregisterListeners(events.Listeners.Where(listener => listener.GetType().Assembly == assembly));

        _containers.Remove(assembly, out var container);
        container?.Dispose();
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
        var instance = serviceProvider.GetService(serviceType) ?? ActivatorUtilities.CreateInstance(serviceProvider, serviceType);

        if (instance is IEventListener listener)
            events.RegisterListeners(listener);

        if (instance is IPlugin plugin)
            RegisterPlugin(plugin);

        return instance;
    }

    public object? GetService(Type serviceType)
    {
        var instance = GetScopedContainer(serviceType.Assembly).GetService(serviceType);

        if (instance is IEventListener listener)
            events.RegisterListeners(listener);

        return instance;
    }

    public TService? GetService<TService>()
    {
        var instance = GetScopedContainer(typeof(TService).Assembly).GetService<TService>();

        if (instance is IEventListener listener)
            events.RegisterListeners(listener);

        return instance;
    }

    public void Register(Action<ServiceCollection> configure, bool activate = true)
    {
        var services = new ServiceCollection();
        configure(services);
        services.RegisterListeners();

        foreach (var service in services)
            serviceProvider.Add(service);

        if (!activate)
            return;

        foreach (var descriptor in services.Where(descriptor => descriptor.Lifetime is ServiceLifetime.Singleton))
            this.GetRequiredService(descriptor.ServiceType);
    }

    private void RegisterPlugin(IPlugin plugin)
    {
        var pluginType = plugin.GetType();

        logger.LogTrace("Injecting {Plugin} into {Name} service collection", plugin.GetType(), plugin.Name);

        // Plugin => Plugin
        serviceProvider.Add(ServiceDescriptor.Singleton(pluginType, plugin));
        this.GetRequiredService(pluginType);
    }

    private IResolverContext GetScopedContainer(Assembly assembly)
    {
        if (!_containers.TryGetValue(assembly, out var child))
        {
            var root = serviceProvider.GetRequiredService<IContainer>();
            child = root.CreateChild(RegistrySharing.Share, assembly);

            // Serilog logger factory not getting shared into a child
            child.RegisterInstance(root.GetRequiredService<ILoggerFactory>());

            _containers.Add(assembly, child);
        }

        return child;
    }
}
