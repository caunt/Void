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
    private readonly Dictionary<Assembly, IContainer> _assemblyContainers = [];
    private readonly Dictionary<Assembly, Dictionary<IPlayer, IContainer>> _assemblyPlayerContainers = [];

    public IServiceProvider Services => CreateCompositeContainer("Transient Composite", [.. _assemblyContainers.Values, container]);

    [Subscribe]
    public static async ValueTask OnProxyStopping(ProxyStoppingEvent _, CancellationToken cancellationToken)
    {
        await File.WriteAllTextAsync("containers.dot", await DryIocTracker.ToGraphStringAsync(), cancellationToken);
    }

    [Subscribe(PostOrder.First)]
    public void OnPlayerConnected(PlayerConnectedEvent @event)
    {
        foreach (var (assembly, container) in _assemblyContainers)
        {
            foreach (var registration in GetScopedServiceRegistrations(container))
            {
                GetContainer(assembly, @event.Player).GetRequiredService(registration.ServiceType);
            }
        }
    }

    [Subscribe(PostOrder.First)]
    public void OnPluginUnloading(PluginUnloadingEvent @event)
    {
        var assembly = @event.Plugin.GetType().Assembly;

        events.UnregisterListeners(events.Listeners.Where(listener => listener.GetType().Assembly == assembly));

        if (_assemblyContainers.Remove(assembly, out var pluginContainer))
        {
            pluginContainer.Untrack();
            pluginContainer.Dispose();
        }

        if (_assemblyPlayerContainers.Remove(assembly, out var playerContainers))
        {
            foreach (var (player, container) in playerContainers)
            {
                container.Untrack();
                container.Dispose();
            }
        }
    }

    [Subscribe]
    public void OnPlayerDisconnected(PlayerDisconnectedEvent @event)
    {
        foreach (var playersContainers in _assemblyPlayerContainers.Values)
        {
            if (!playersContainers.Remove(@event.Player, out var container))
                continue;

            foreach (var service in GetScopedServices(@event.Player))
            {
                if (service is IEventListener listener)
                    events.UnregisterListeners(listener);
            }

            container.Untrack();
            container.Dispose();
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
        var instance = container.GetService(serviceType) ?? ActivatorUtilities.CreateInstance(Services, serviceType);

        if (instance is IEventListener listener)
            events.RegisterListeners(listener);

        if (instance is IPlugin plugin)
            RegisterPlugin(plugin);

        return instance;
    }

    public object? GetService(Type serviceType)
    {
        return Services.GetService(serviceType);
    }

    public TService? GetService<TService>()
    {
        return Services.GetService<TService>();
    }

    public void Register(Action<ServiceCollection> configure, bool activate = true)
    {
        var services = new ServiceCollection();
        configure(services);
        services.RegisterListeners();

        foreach (var service in services)
        {
            var serviceType = service.ServiceType;
            var assembly = service.ServiceType.Assembly;

            // TODO: ServiceType.IsCollectible does not guarantee that the service is from plugin assembly
            // ServiceType might be just an interface from proxy itself, and plugin less likely, but might be not collectible
            if (assembly.IsCollectible)
                GetContainer(assembly).Add(service);
            else
                container.Add(service);

            if (!activate)
                continue;

            if (service.Lifetime is ServiceLifetime.Singleton)
            {
                GetContainer(assembly).GetRequiredService(service.ServiceType);
            }
            else if (service.Lifetime is ServiceLifetime.Scoped)
            {
                var players = container.GetRequiredService<IPlayerService>();

                foreach (var player in players.All)
                {
                    if (service.ServiceType.ContainsGenericParameters)
                        continue;

                    GetContainer(assembly, player).GetRequiredService(service.ServiceType);
                }
            }
        }
    }

    private void RegisterPlugin(IPlugin plugin)
    {
        var pluginType = plugin.GetType();

        logger.LogTrace("Injecting {Plugin} into {Name} service collection", plugin.GetType(), plugin.Name);

        // Plugin => Plugin
        GetContainer(pluginType.Assembly).Add(ServiceDescriptor.Singleton(pluginType, plugin));
        GetContainer(pluginType.Assembly).GetRequiredService(pluginType);
    }

    private ListeningServiceProvider GetContainer(Assembly assembly)
    {
        if (!_assemblyContainers.TryGetValue(assembly, out var child))
        {
            child = CreateCompositeContainer($"{assembly.GetName().Name} Composite", _assemblyPlayerContainers.SelectMany(pair => pair.Value.Select(pair => pair.Value)).Concat(_assemblyContainers.Values).Append(container));

            // Serilog logger factory not getting shared into a child
            child.RegisterInstance(container.GetRequiredService<ILoggerFactory>());

            _assemblyContainers.Add(assembly, child);
        }

        return new ListeningServiceProvider(child);
    }

    private ListeningServiceProvider GetContainer(Assembly assembly, IPlayer player)
    {
        if (!_assemblyPlayerContainers.TryGetValue(assembly, out var playerContainers))
        {
            playerContainers = [];
            _assemblyPlayerContainers.Add(assembly, playerContainers);
        }

        if (!playerContainers.TryGetValue(player, out var assemblyContainer))
        {
            assemblyContainer = CreateCompositeContainer("Player Composite", _assemblyPlayerContainers.SelectMany(pair => pair.Value.Select(pair => pair.Value)).Concat(_assemblyContainers.Values).Append(container));

            // IPlayer
            assemblyContainer.RegisterDelegate(request => player);

            playerContainers.Add(player, assemblyContainer);
        }

        // Ensure all scoped services are registered in player container
        var source = GetContainer(assembly).Source.GetRequiredService<IContainer>();

        foreach (var registration in GetScopedServiceRegistrations(source))
        {
            if (assemblyContainer.IsRegistered(registration.ServiceType, registration.OptionalServiceKey))
                continue;

            var registrationFactory = registration.Factory;

            if (registration.ImplementationType is not null)
            {
                assemblyContainer.Register(
                    registration.ServiceType,
                    registration.ImplementationType,
                    registrationFactory.Reuse,
                    registrationFactory.Made,
                    serviceKey: registration.OptionalServiceKey);
            }
            else
            {
                assemblyContainer.Register(
                    registrationFactory,
                    registration.ServiceType,
                    registration.OptionalServiceKey,
                    IfAlreadyRegistered.Throw,
                    false);
            }
        }

        return new ListeningServiceProvider(assemblyContainer);
    }

    private Container CreateCompositeContainer(string name, params IEnumerable<IContainer> containers)
    {
        var compositeContainer = new Container(container.Rules
            .WithUnknownServiceResolvers(request =>
            {
                return DelegateFactory.Of(context =>
                {
                    foreach (var childContainer in containers)
                    {
                        var serviceType = request.ServiceType;

                        // Open generic types like ILogger<Something> to ILogger<>
                        if (serviceType.IsGenericType)
                            serviceType = serviceType.GetGenericTypeDefinition();

                        if (!childContainer.IsRegistered(serviceType, request.ServiceKey))
                            continue;

                        var instance = childContainer.Resolve(request.ServiceType, request.ServiceKey, IfUnresolved.ReturnDefaultIfNotRegistered);

                        if (instance is null)
                            continue;

                        return instance;
                    }

                    return null;
                });
            }));

        container.Track("App Root Container");
        compositeContainer.Track(name);

        return compositeContainer;
    }

    private IEnumerable<object> GetScopedServices(IPlayer player)
    {
        foreach (var (assembly, playersContainers) in _assemblyPlayerContainers)
        {
            foreach (var (scopedPlayer, container) in playersContainers)
            {
                if (scopedPlayer != player)
                    continue;

                foreach (var service in GetScopedServices(container))
                    yield return service;
            }
        }
    }

    private static IEnumerable<object> GetScopedServices(IContainer container)
    {
        foreach (var registration in GetScopedServiceRegistrations(container))
        {
            if (container.GetService(registration.ServiceType) is not { } instance)
                continue;

            yield return instance;
        }
    }

    private static IEnumerable<ServiceRegistrationInfo> GetScopedServiceRegistrations(IContainer container)
    {
        foreach (var registration in container.GetServiceRegistrations())
        {
            var lifespan = registration.Factory.Reuse.Lifespan;

            if (lifespan <= Reuse.Transient.Lifespan || lifespan >= Reuse.Singleton.Lifespan)
                continue;

            var serviceType = registration.ServiceType;

            // Open generic types like ILogger<Something> to ILogger<>
            if (serviceType.IsGenericType)
                serviceType = serviceType.GetGenericTypeDefinition();

            yield return registration;
        }
    }
}
