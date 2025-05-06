using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using DryIoc;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Plugins;
using Void.Proxy.Api.Events.Proxy;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Players.Contexts;
using Void.Proxy.Api.Plugins;
using Void.Proxy.Api.Plugins.Dependencies;
using Void.Proxy.Plugins.Dependencies.Extensions;
using Void.Proxy.Utils;

namespace Void.Proxy.Plugins.Dependencies;

public class DependencyService(ILogger<DependencyService> logger, IContainer container) : IDependencyService
{
    private readonly Dictionary<Assembly, IContainer> _assemblyContainers = [];
    private readonly Dictionary<Assembly, Dictionary<int, IContainer>> _assemblyPlayerContainers = [];

    [Subscribe]
    public static async ValueTask OnProxyStopping(ProxyStoppingEvent _, CancellationToken cancellationToken)
    {
        await File.WriteAllTextAsync("containers.dot", await DryIocTracker.ToGraphStringAsync(), cancellationToken);
    }

    [Subscribe(PostOrder.First)]
    public void OnPluginUnloading(PluginUnloadingEvent @event)
    {
        var assembly = @event.Plugin.GetType().Assembly;

        var events = container.GetRequiredService<IEventService>();
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

    public TService CreateInstance<TService>(CancellationToken cancellationToken = default)
    {
        return CreateInstance<TService>(typeof(TService), cancellationToken);
    }

    public TService CreateInstance<TService>(Type serviceType, CancellationToken cancellationToken = default)
    {
        return (TService?)CreateInstance(serviceType, cancellationToken) ??
            throw new InvalidOperationException($"Unable to cast instance of {serviceType} to {typeof(TService)}");
    }

    public object CreateInstance(Type serviceType, CancellationToken cancellationToken = default)
    {
        if (serviceType.IsAssignableTo(typeof(IPlugin)))
            RegisterPlugin(serviceType, cancellationToken);

        var instance = ActivatorUtilities.GetServiceOrCreateInstance(GetCompositeSortedBy(serviceType.Assembly), serviceType);

        // Since all containers might not have this service type, register it manually
        if (instance is IEventListener listener)
        {
            var events = container.Resolve<IEventService>();
            events.RegisterListeners(cancellationToken, listener);
        }

        return instance;
    }

    public bool TryGetScopedPlayerContext(object instance, [MaybeNullWhen(false)] out IPlayerContext context)
    {
        var serviceType = instance.GetType();

        foreach (var playersContainer in _assemblyPlayerContainers.Values)
        {
            foreach (var (playerStableHashCode, container) in playersContainer)
            {
                if (!container.IsRegistered(serviceType))
                    continue;

                if (container.Resolve(serviceType) != instance)
                    continue;

                context = container.Resolve<IPlayerContext>();
                return true;
            }
        }

        context = null;
        return false;
    }

    public IServiceProvider CreatePlayerComposite(IPlayer player)
    {
        var composite = CreateCompositeContainer($"[{player}] Player Composite",
            _assemblyPlayerContainers.SelectMany(pair => pair.Value.Where(pair => pair.Key == player.GetStableHashCode()).Select(pair => pair.Value))
            .Append(container)
            .Concat(_assemblyContainers.Values));

        return ListeningServiceProvider.Wrap(composite, default);
    }

    public void ActivatePlayerContext(IPlayerContext context)
    {
        foreach (var (assembly, container) in _assemblyContainers)
        {
            foreach (var registration in GetServiceRegistrations(container))
            {
                GetContainer(assembly, context.Player).GetRequiredService(registration.ServiceType);
            }
        }
    }

    public void DisposePlayerContext(IPlayerContext context)
    {
        var foundContainer = false;

        foreach (var playersContainers in _assemblyPlayerContainers.Values)
        {
            if (!playersContainers.Remove(context.Player.GetStableHashCode(), out var container))
                continue;

            foreach (var service in GetServices(container))
            {
                if (service is IEventListener listener)
                {
                    var events = container.GetRequiredService<IEventService>();
                    events.UnregisterListeners(listener);
                }
            }

            container.Untrack();
            container.Dispose();

            foundContainer = true;
        }

        var scopedComposite = context.Services.GetRequiredService<IContainer>();

        scopedComposite.Untrack();
        scopedComposite.Dispose();

        if (!foundContainer)
            logger.LogWarning("No container found when disconnecting player {Player}", context.Player);
    }

    public object? GetService(Type serviceType)
    {
        return ListeningServiceProvider.Wrap(GetCompositeSortedBy(serviceType.Assembly), default).GetService(serviceType);
    }

    public TService? GetService<TService>()
    {
        return ListeningServiceProvider.Wrap(GetCompositeSortedBy(typeof(TService).Assembly), default).GetService<TService>();
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

    private void RegisterPlugin(Type pluginType, CancellationToken cancellationToken)
    {
        logger.LogTrace("Injecting {PluginType}", pluginType);

        var assembly = pluginType.Assembly;
        var container = GetContainer(assembly, cancellationToken);

        // Plugin => Plugin
        container.Add(ServiceDescriptor.Singleton(pluginType, pluginType));
        container.Add(ServiceDescriptor.Singleton(provider => (IPlugin)provider.GetRequiredService(pluginType)));
        container.Add(ServiceDescriptor.Singleton(provider => provider.GetRequiredService<ILoggerFactory>().CreateLogger(pluginType.Name)));
        container.GetRequiredService(pluginType);
    }

    private ListeningServiceProvider GetContainer(Assembly assembly, CancellationToken cancellationToken = default)
    {
        if (!_assemblyContainers.TryGetValue(assembly, out var child))
        {
            child = CreateCompositeContainer($"[{assembly.GetName().Name}] Assembly Composite", _assemblyContainers.Values.Append(container));
            _assemblyContainers.Add(assembly, child);
        }

        return new ListeningServiceProvider(child, cancellationToken);
    }

    private ListeningServiceProvider GetContainer(Assembly assembly, IPlayer player)
    {
        if (!_assemblyPlayerContainers.TryGetValue(assembly, out var playerContainers))
        {
            playerContainers = [];
            _assemblyPlayerContainers.Add(assembly, playerContainers);
        }

        if (!playerContainers.TryGetValue(player.GetStableHashCode(), out var playerContainer))
        {
            playerContainer = CreateCompositeContainer($"[{assembly.GetName().Name}/{player}] Assembly Player Composite", _assemblyPlayerContainers.Values
                .SelectMany(playersContainers => playersContainers.Where(pair => pair.Key == player.GetStableHashCode()).Select(pair => pair.Value))
                .Append(container)
                .Concat(_assemblyContainers.Values));

            playerContainer.RegisterInstance(player.Context, setup: Setup.With(preventDisposal: true));
            playerContainers.Add(player.GetStableHashCode(), playerContainer);
        }

        // Ensure all scoped services are registered in player container
        var assemblyContainer = GetContainer(assembly);
        var source = assemblyContainer.Source.GetRequiredService<IContainer>();

        foreach (var registration in GetServiceRegistrations(source))
        {
            var registrationFactory = registration.Factory;
            var reuse = registrationFactory.Reuse;

            // Register only Scoped services in player own "scoped" container
            if (reuse.Lifespan <= Reuse.Transient.Lifespan || reuse.Lifespan >= Reuse.Singleton.Lifespan)
                continue;

            if (playerContainer.IsRegistered(registration.ServiceType, registration.OptionalServiceKey))
                continue;

            // Switch Scoped to Singleton, as it is now registered in its own "scoped" container
            reuse = Reuse.Singleton;

            if (registration.ImplementationType is not null)
            {
                playerContainer.Register(
                    registration.ServiceType,
                    registration.ImplementationType,
                    reuse,
                    registrationFactory.Made,
                    serviceKey: registration.OptionalServiceKey);
            }
            else
            {
                // TODO: Bug, need to change reuse of factory to singleton
                playerContainer.Register(
                    registrationFactory,
                    registration.ServiceType,
                    registration.OptionalServiceKey,
                    IfAlreadyRegistered.Throw,
                    false);
            }
        }

        return new ListeningServiceProvider(playerContainer, assemblyContainer.CancellationToken);
    }

    private Container GetCompositeSortedBy(Assembly assembly)
    {
        var containers = _assemblyContainers.Values.AsEnumerable();

        if (_assemblyContainers.TryGetValue(assembly, out var preferredContainer))
            containers = containers.OrderByDescending(container => container == preferredContainer);

        return CreateCompositeContainer("Transient Composite", [.. containers, container]);
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
            var lifespan = registration.Factory.Reuse?.Lifespan ?? Reuse.Singleton.Lifespan;

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
