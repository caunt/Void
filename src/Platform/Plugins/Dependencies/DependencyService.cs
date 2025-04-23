using System.Reflection;
using DryIoc;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Player;
using Void.Proxy.Api.Events.Plugins;
using Void.Proxy.Api.Events.Proxy;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Extensions;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Plugins;
using Void.Proxy.Api.Plugins.Dependencies;
using Void.Proxy.Utils;

namespace Void.Proxy.Plugins.Dependencies;

public class DependencyService(ILogger<DependencyService> logger, IContainer container, IEventService events) : IDependencyService
{
    private readonly Dictionary<Assembly, IContainer> _containers = [];

    [Subscribe]
    public async ValueTask OnProxyStopping(ProxyStoppingEvent @event, CancellationToken cancellationToken)
    {
        await File.WriteAllTextAsync("containers.dot", await DryIocTracker.ToGraphStringAsync(), cancellationToken);
    }

    [Subscribe(PostOrder.First)]
    public void OnPluginUnloading(PluginUnloadingEvent @event)
    {
        var assembly = @event.Plugin.GetType().Assembly;

        container.Remove(descriptor => descriptor.ServiceType.Assembly == assembly);
        events.UnregisterListeners(events.Listeners.Where(listener => listener.GetType().Assembly == assembly));

        // Do not dispose container, as it does share registrations and resolutions cache with root
        if (_containers.Remove(assembly, out var pluginContainer))
            pluginContainer.Untrack();

        var players = container.GetRequiredService<IPlayerService>();

        foreach (var player in players.All)
        {
            var playerContainer = player.Context.Services.GetRequiredService<IContainer>();

            if (playerContainer == null)
                continue;

            playerContainer.Remove(descriptor => descriptor.ServiceType.Assembly == assembly);
        }
    }

    [Subscribe(PostOrder.First)]
    public static void OnPlayerConnected(PlayerConnectedEvent @event)
    {
        var container = @event.Player.Context.Services.GetRequiredService<IContainer>();

        foreach (var registration in container.GetServiceRegistrations())
        {
            if (registration.ServiceType.ContainsGenericParameters)
                continue;

            var reuse = registration.Factory.Reuse;

            // Is not 'Scoped?
            if (reuse.Lifespan <= Reuse.Transient.Lifespan || reuse.Lifespan >= Reuse.Singleton.Lifespan)
                continue;

            @event.Player.Context.Services.GetRequiredService(registration.ServiceType);
        }
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
        var instance = container.GetService(serviceType) ?? ActivatorUtilities.CreateInstance(container, serviceType);

        if (instance is IEventListener listener)
            events.RegisterListeners(listener);

        if (instance is IPlugin plugin)
            RegisterPlugin(plugin);

        return instance;
    }

    public object? GetService(Type serviceType)
    {
        return GetScoped(serviceType.Assembly).GetService(serviceType);
    }

    public TService? GetService<TService>()
    {
        return GetScoped(typeof(TService).Assembly).GetService<TService>();
    }

    public void Register(Action<ServiceCollection> configure, bool activate = true)
    {
        var services = new ServiceCollection();
        configure(services);
        services.RegisterListeners();

        foreach (var service in services)
            container.Add(service);

        if (!activate)
            return;

        var players = container.GetRequiredService<IPlayerService>();

        foreach (var descriptor in services)
        {
            if (descriptor.Lifetime is ServiceLifetime.Singleton)
            {
                GetScoped(descriptor.ServiceType.Assembly).GetRequiredService(descriptor.ServiceType);
            }
            else if (descriptor.Lifetime is ServiceLifetime.Scoped)
            {
                foreach (var player in players.All)
                {
                    player.Context.Services.GetRequiredService(descriptor.ServiceType);
                }
            }
        }
    }

    private void RegisterPlugin(IPlugin plugin)
    {
        var pluginType = plugin.GetType();

        logger.LogTrace("Injecting {Plugin} into {Name} service collection", plugin.GetType(), plugin.Name);

        // Plugin => Plugin
        container.Add(ServiceDescriptor.Singleton(pluginType, plugin));
        this.GetRequiredService(pluginType);
    }

    private ListeningServiceProvider GetScoped(Assembly assembly)
    {
        if (!_containers.TryGetValue(assembly, out var child))
        {
            child = container.CreateChild(RegistrySharing.Share, assembly);

            container.Track("App Root Container");
            child.Track($"[{assembly.GetName().Name}] Assembly Container", container);

            // Serilog logger factory not getting shared into a child
            child.RegisterInstance(container.GetRequiredService<ILoggerFactory>());

            _containers.Add(assembly, child);
        }

        return new ListeningServiceProvider(child);
    }
}
