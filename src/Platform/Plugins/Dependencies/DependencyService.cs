using System.Reflection;
using DryIoc;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Plugins;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Players.Contexts;
using Void.Proxy.Api.Plugins;
using Void.Proxy.Api.Plugins.Dependencies;
using Void.Proxy.Plugins.Dependencies.Extensions;

namespace Void.Proxy.Plugins.Dependencies;

public class DependencyService(ILogger<DependencyService> logger, IContainer rootContainer) : IDependencyService
{
    private readonly Dictionary<Assembly, PluginContainer> _plugins = [];

    public TService CreateInstance<TService>(CancellationToken cancellationToken = default, params object[] parameters)
    {
        return CreateInstance<TService>(typeof(TService), cancellationToken, parameters);
    }

    public TService CreateInstance<TService>(Type serviceType, CancellationToken cancellationToken = default, params object[] parameters)
    {
        return (TService?)CreateInstance(serviceType, cancellationToken, parameters) ??
               throw new InvalidOperationException($"Unable to cast instance of {serviceType} to {typeof(TService)}");
    }

    public object CreateInstance(Type serviceType, CancellationToken cancellationToken = default, params object[] parameters)
    {
        var assembly = serviceType.Assembly;

        if (serviceType.IsAssignableTo(typeof(IPlugin)))
        {
            logger.LogTrace("Injecting {PluginType}", serviceType);

            var pluginContainers = GetPluginContainer(assembly, cancellationToken);

            // Plugin => Plugin
            pluginContainers.Root.Add(ServiceDescriptor.Singleton(serviceType, serviceType));
            pluginContainers.Root.Add(ServiceDescriptor.Singleton(provider => (IPlugin)provider.GetRequiredService(serviceType)));
            pluginContainers.Root.Add(ServiceDescriptor.Singleton(provider => GetEntryPoint(assembly).GetRequiredService<ILoggerFactory>().CreateLogger(serviceType.Name)));

            GetEntryPoint(preferredAssembly: assembly).GetRequiredService(serviceType);
        }

        var composite = GetEntryPoint(preferredAssembly: assembly);
        var instance = composite.GetService(serviceType) ?? ActivatorUtilities.CreateInstance(composite, serviceType, parameters);

        if (instance is not IEventListener listener)
            return instance;

        var events = rootContainer.Resolve<IEventService>();
        events.RegisterListeners(cancellationToken, listener);

        return instance;
    }

    public void Register(Action<ServiceCollection> configure, bool activate = true)
    {
        var services = new ServiceCollection();
        configure(services);

        var assembly = configure.Method.DeclaringType?.Assembly ?? GetType().Assembly;

        foreach (var service in services)
            GetPluginContainer(assembly).Root.Add(service);

        if (!activate)
            return;

        foreach (var service in services)
        {
            if (service.Lifetime is ServiceLifetime.Transient)
                continue;

            var serviceType = service.ServiceType;

            if (serviceType.IsOpenGeneric())
                continue;

            switch (service.Lifetime)
            {
                case ServiceLifetime.Transient:
                    // Not activated
                    break;
                case ServiceLifetime.Scoped:
                    {
                        var players = rootContainer.GetRequiredService<IPlayerService>();

                        foreach (var player in players.All)
                            GetEntryPoint(player, assembly).GetRequiredService(serviceType);
                        break;
                    }
                case ServiceLifetime.Singleton:
                    GetEntryPoint(assembly).GetRequiredService(serviceType);
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported service lifetime: {service.Lifetime}");
            }
        }
    }

    public object? GetService(Type serviceType)
    {
        return GetEntryPoint(preferredAssembly: serviceType.Assembly).GetService(serviceType);
    }

    public TService? GetService<TService>()
    {
        return GetEntryPoint(preferredAssembly: typeof(TService).Assembly).GetService<TService>();
    }

    public void ActivatePlayerScope(IPlayerContext context)
    {
        var playerStableHashCode = context.Player.GetStableHashCode();
        var queues = new Queue<Assembly>(_plugins.Keys);

        while (queues.TryDequeue(out var assembly))
        {
            var playerScope = GetPlayerScope(assembly, playerStableHashCode, context);

            foreach (var registration in playerScope.GetRequiredService<IContainer>().GetServiceRegistrations())
            {
                if (registration.ServiceType.IsOpenGeneric())
                    continue;

                if (registration.Factory?.Reuse is not CurrentScopeReuse)
                    continue;

                _ = GetEntryPoint(context.Player, assembly).GetRequiredService(registration.ServiceType);
            }
        }
    }

    public void DisposePlayerScope(IPlayerContext context)
    {
        var events = rootContainer.Resolve<IEventService>();
        var playerStableHashCode = context.Player.GetStableHashCode();

        foreach (var plugin in _plugins.Values)
        {
            if (!plugin.Scopes.Remove(playerStableHashCode, out var resolverContext))
                continue;

            foreach (var scope in resolverContext.CurrentScope)
            {
                foreach (var service in scope.GetSnapshotOfServicesWithFactoryIDs())
                {
                    if (service.Value is not IEventListener listener)
                        continue;

                    events.UnregisterListeners(listener);
                }
            }

            resolverContext.Dispose();
        }
    }

    public bool TryGetServiceReuse(Type serviceType, out ServiceLifetime reuse)
    {
        var pluginContainer = GetPluginContainer(serviceType.Assembly);
        var container = pluginContainer.Root.GetRequiredService<IContainer>();
        var registration = container.GetServiceRegistrations().FirstOrDefault(registration => registration.ServiceType == serviceType);
        var registered = registration.Factory is not null;

        reuse = registration.Factory?.Reuse switch
        {
            SingletonReuse => ServiceLifetime.Singleton,
            CurrentScopeReuse => ServiceLifetime.Scoped,
            _ => ServiceLifetime.Transient
        };

        return registered;
    }

    public IServiceProvider GetEntryPoint(IPlayer player, Assembly? preferredAssembly = null)
    {
        var playerStableHashCode = player.GetStableHashCode();
        EnsurePlayerScopes();

        return CombineLazy(GetScopedContainers, GetSingletonContainers);

        IEnumerable<IServiceProvider> GetScopedContainers()
        {
            foreach (var assembly in GetPluginAssemblies())
            {
                if (!_plugins.TryGetValue(assembly, out var pluginContainer))
                    continue;

                if (!pluginContainer.Scopes.TryGetValue(playerStableHashCode, out var playerScope))
                    continue;

                yield return playerScope;
            }

            yield return rootContainer;

            foreach (var assembly in GetPluginAssemblies())
                yield return GetPluginContainer(assembly).Root;
        }

        IEnumerable<IServiceProvider> GetSingletonContainers()
        {
            yield return rootContainer;

            foreach (var assembly in GetPluginAssemblies())
                yield return GetPluginContainer(assembly).Root;
        }

        IEnumerable<Assembly> GetPluginAssemblies()
        {
            var assemblies = _plugins.Keys.AsEnumerable();

            if (preferredAssembly is not null)
                assemblies = assemblies.OrderByDescending(assembly => assembly == preferredAssembly);

            return assemblies;
        }

        void EnsurePlayerScopes()
        {
            foreach (var assembly in _plugins.Keys)
                GetPlayerScope(assembly, playerStableHashCode, player.Context);
        }
    }

    public IServiceProvider GetEntryPoint(Assembly? preferredAssembly = null)
    {
        return CombineLazy(GetRootContainers);

        IEnumerable<IServiceProvider> GetRootContainers()
        {
            yield return rootContainer;

            foreach (var assembly in GetPluginAssemblies())
                yield return GetPluginContainer(assembly).Root;
        }

        IEnumerable<Assembly> GetPluginAssemblies()
        {
            var assemblies = _plugins.Keys.AsEnumerable();

            if (preferredAssembly is not null)
                assemblies = assemblies.OrderByDescending(assembly => assembly == preferredAssembly);

            return assemblies;
        }
    }

    [Subscribe(PostOrder.First)]
    public void OnPluginUnloading(PluginUnloadingEvent @event)
    {
        var assembly = @event.Plugin.GetType().Assembly;

        var events = rootContainer.GetRequiredService<IEventService>();
        events.UnregisterListeners(events.Listeners.Where(listener => listener.GetType().Assembly == assembly));

        if (!_plugins.Remove(assembly, out var container))
            return;

        container.Root.GetRequiredService<IContainer>().Dispose();

        foreach (var (playerStableHashCode, scope) in container.Scopes)
            scope.Dispose();
    }

    private IServiceProvider GetPlayerScope(Assembly assembly, int playerStableHashCode, IPlayerContext context)
    {
        var container = GetPluginContainer(assembly);

        if (container.Scopes.TryGetValue(playerStableHashCode, out var playerScope))
            return playerScope;

        playerScope = GetPluginContainer(assembly).Root.GetRequiredService<IContainer>().OpenScope(nameof(IPlayer));
        container.Scopes[playerStableHashCode] = playerScope;

        playerScope.Add(ServiceDescriptor.Singleton(context));

        return playerScope;
    }

    private PluginContainer GetPluginContainer(Assembly pluginAssembly, CancellationToken cancellationToken = default)
    {
        if (_plugins.TryGetValue(pluginAssembly, out var pluginContainer))
            return pluginContainer;

        var emptyContainer = Combine();
        emptyContainer.Add(ServiceDescriptor.Singleton<IServiceScopeFactory>(serviceProvider => new DependencyServiceScopeFactory((IResolverContext)serviceProvider)));
        _plugins.Add(pluginAssembly, pluginContainer = new PluginContainer(Root: emptyContainer, Scopes: []));

        return pluginContainer;
    }

    private Container Combine(params IServiceProvider[] containers)
    {
        return CombineLazy(() => containers);
    }

    private Container CombineLazy(Func<IEnumerable<IServiceProvider>> getContainers, Func<IEnumerable<IServiceProvider>>? getSingletonContainers = null)
    {
        var events = rootContainer.Resolve<IEventService>();
        var getSingletonResolutionContainers = getSingletonContainers ?? getContainers;

        return new Container(rootContainer.Rules.WithUnknownServiceResolvers(ResolveFactory));

        InstanceFactory? ResolveFactory(Request request)
        {
            var isSingletonResolution =
                (request.Flags & RequestFlags.IsSingletonOrDependencyOfSingleton) != 0 ||
                IsSingletonService(request.ServiceType);

            var service = ResolveService(request.ServiceType, isSingletonResolution ? getSingletonResolutionContainers : getContainers);

            if (service is null)
                return null;

            return new InstanceFactory(service);
        }

        bool IsSingletonService(Type serviceType)
        {
            foreach (var serviceProvider in getSingletonResolutionContainers())
            {
                var container = serviceProvider.GetRequiredService<IContainer>();

                foreach (var registration in container.GetServiceRegistrations())
                {
                    if (!Matches(serviceType, registration.ServiceType))
                        continue;

                    if (registration.Factory?.Reuse is SingletonReuse)
                        return true;
                }
            }

            return false;
        }

        static bool Matches(Type requestedServiceType, Type registeredServiceType)
        {
            if (requestedServiceType == registeredServiceType)
                return true;

            if (!requestedServiceType.IsConstructedGenericType)
                return false;

            return requestedServiceType.GetGenericTypeDefinition() == registeredServiceType;
        }

        object? ResolveService(Type serviceType, Func<IEnumerable<IServiceProvider>> getServiceProviders)
        {
            foreach (var serviceProvider in getServiceProviders())
            {
                var container = serviceProvider.GetRequiredService<IContainer>();

                if (!container.CanGetService(serviceType))
                    continue;

                var configuredContainer = container.With(dependencyRules => dependencyRules.WithUnknownServiceResolvers(ResolveFactory));

                var resolver = serviceProvider is IResolverContext { CurrentScope: not null } resolverContext
                    ? configuredContainer.WithCurrentScope(resolverContext.CurrentScope)
                    : configuredContainer;

                var service = resolver.Resolve(serviceType, IfUnresolved.ReturnDefault);

                if (service is null)
                    continue;

                if (service is IEventListener listener)
                    events.RegisterListeners(listener);

                return service;
            }

            return null;
        }
    }

    private sealed class DependencyServiceScopeFactory(IResolverContext resolverContext) : IServiceScopeFactory
    {
        public IServiceScope CreateScope()
        {
            return new DependencyServiceScope(resolverContext.OpenScope());
        }
    }

    private sealed class DependencyServiceScope(IResolverContext resolverContext) : IServiceScope
    {
        public IServiceProvider ServiceProvider => resolverContext;

        public void Dispose()
        {
            resolverContext.Dispose();
        }
    }

    private record PluginContainer(IServiceProvider Root, Dictionary<int, IResolverContext> Scopes);
}
