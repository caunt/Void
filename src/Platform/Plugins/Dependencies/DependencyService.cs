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
    private readonly Dictionary<Assembly, Dictionary<int, IContainer>> _assemblyPlayerContainers = [];

    public IServiceProvider Services => CreateCompositeContainer("Transient Composite", [.. _assemblyContainers.Values, container]);

    [Subscribe]
    public static async ValueTask OnProxyStopping(ProxyStoppingEvent _, CancellationToken cancellationToken)
    {
        await File.WriteAllTextAsync("containers.dot", await DryIocTracker.ToGraphStringAsync(), cancellationToken);
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
        var foundContainer = false;

        foreach (var playersContainers in _assemblyPlayerContainers.Values)
        {
            if (!playersContainers.Remove(@event.Player.GetStableHashCode(), out var container))
                continue;

            foreach (var service in GetServices(container))
            {
                if (service is IEventListener listener)
                    events.UnregisterListeners(listener);
            }

            container.Untrack();
            container.Dispose();

            foundContainer = true;
        }

        if (!foundContainer)
            logger.LogWarning("No container found when disconnecting player {Player}", @event.Player);
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

    public IServiceProvider CreatePlayerComposite(IPlayer player)
    {
        // Ensure scoped services instantiated
        foreach (var (assembly, container) in _assemblyContainers)
        {
            foreach (var registration in GetServiceRegistrations(container))
            {
                GetContainer(assembly, player).GetRequiredService(registration.ServiceType);
            }
        }

        var composite = CreateCompositeContainer($"[{player}] Player Composite",
            _assemblyPlayerContainers.SelectMany(pair => pair.Value.Where(pair => pair.Key == player.GetStableHashCode()).Select(pair => pair.Value))
            .Append(container));

        return ListeningServiceProvider.Wrap(composite);
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

        foreach (var service in services)
        {
            var serviceType = service.ServiceType;
            var assembly = service.ServiceType.Assembly;

            GetContainer(assembly).Add(service);

            if (!activate)
                continue;

            if (service.Lifetime is ServiceLifetime.Singleton)
            {
                GetContainer(assembly).GetRequiredService(serviceType);
            }
            else if (service.Lifetime is ServiceLifetime.Scoped)
            {
                var players = container.GetRequiredService<IPlayerService>();

                foreach (var player in players.All)
                {
                    if (serviceType.ContainsGenericParameters)
                        continue;

                    GetContainer(assembly, player).GetRequiredService(serviceType);
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
            child = CreateCompositeContainer($"[{assembly.GetName().Name}] Assembly Composite",
                _assemblyPlayerContainers.SelectMany(pair => pair.Value.Select(pair => pair.Value))
                .Concat(_assemblyContainers.Values)
                .Append(container));

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

        if (!playerContainers.TryGetValue(player.GetStableHashCode(), out var assemblyContainer))
        {
            assemblyContainer = CreateCompositeContainer($"[{assembly.GetName().Name}/{player}] Assembly Player Composite",
                _assemblyPlayerContainers.SelectMany(pair => pair.Value.Select(pair => pair.Value))
                .Concat(_assemblyContainers.Values)
                .Append(container));

            // IPlayer
            assemblyContainer.RegisterDelegate(request => player, Reuse.Singleton);

            playerContainers.Add(player.GetStableHashCode(), assemblyContainer);
        }

        // Ensure all scoped services are registered in player container
        var source = GetContainer(assembly).Source.GetRequiredService<IContainer>();

        foreach (var registration in GetServiceRegistrations(source))
        {
            var registrationFactory = registration.Factory;
            var reuse = registrationFactory.Reuse;

            // Register only Scoped services in player own "scoped" container
            if (reuse.Lifespan <= Reuse.Transient.Lifespan || reuse.Lifespan >= Reuse.Singleton.Lifespan)
                continue;

            if (assemblyContainer.IsRegistered(registration.ServiceType, registration.OptionalServiceKey))
                continue;

            // Switch Scoped to Singleton, as it is now registered in its own "scoped" container
            reuse = Reuse.Singleton;

            if (registration.ImplementationType is not null)
            {
                assemblyContainer.Register(
                    registration.ServiceType,
                    registration.ImplementationType,
                    reuse,
                    registrationFactory.Made,
                    serviceKey: registration.OptionalServiceKey);
            }
            else
            {
                // TODO: Bug, need to change reuse of factory to singleton
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
                var serviceKey = request.ServiceKey;
                var serviceTypeClosedGeneric = request.ServiceType;
                var serviceTypeOpenGeneric = serviceTypeClosedGeneric;

                // Open generic types like ILogger<Something> to ILogger<>
                if (serviceTypeOpenGeneric.IsGenericType)
                    serviceTypeOpenGeneric = serviceTypeOpenGeneric.GetGenericTypeDefinition();

                return DelegateFactory.Of(context =>
                {
                    foreach (var childContainer in containers)
                    {
                        if (!childContainer.IsRegistered(serviceTypeOpenGeneric, serviceKey))
                            continue;

                        // TODO: Add OptionalServiceKey?
                        var instance = childContainer.GetService(serviceTypeClosedGeneric);

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

    private IEnumerable<object> GetServices(IContainer container)
    {
        foreach (var registration in GetServiceRegistrations(container))
        {
            var serviceType = registration.ServiceType;

            // Open generic types like ILogger<Something> to ILogger<>
            if (serviceType.IsGenericType)
                serviceType = serviceType.GetGenericTypeDefinition();

            if (container.GetService(serviceType) is not { } instance)
                continue;

            yield return instance;
        }
    }

    private IEnumerable<ServiceRegistrationInfo> GetServiceRegistrations(IContainer container)
    {
        // Scoped services are allowed in non scoped containers, so they can later be copied to "scoped" player containers as singletons
        var allowScoped = _assemblyContainers.ContainsValue(container);

        foreach (var registration in container.GetServiceRegistrations())
        {
            var lifespan = registration.Factory.Reuse.Lifespan;

            if (lifespan <= Reuse.Transient.Lifespan)
                continue;

            if (!allowScoped)
            {
                if (lifespan < Reuse.Singleton.Lifespan)
                    throw new InvalidOperationException($"Scoped service registrations are not allowed ({registration.ServiceType}) since Scoped services registered as Singletons in corresponding player containers.");
            }

            yield return registration;
        }
    }
}
