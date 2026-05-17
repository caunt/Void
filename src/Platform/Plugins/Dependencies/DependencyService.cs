using System.Diagnostics.CodeAnalysis;
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

        foreach (var service in services)
            GetPluginContainer(service.ServiceType.Assembly).Root.Add(service);

        if (!activate)
            return;

        foreach (var service in services)
        {
            if (service.Lifetime is ServiceLifetime.Transient)
                continue;

            var serviceType = service.ServiceType;

            if (serviceType.IsOpenGeneric())
                continue;

            var assembly = service.ServiceType.Assembly;
            
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
        
        foreach (var (assembly, container) in _plugins)
        {
            var playerScope = GetPlayerScope(assembly, playerStableHashCode, () => context);
            
            foreach (var registration in playerScope.GetRequiredService<IContainer>().GetServiceRegistrations())
            {
                if (registration.ServiceType.IsOpenGeneric())
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

    public IServiceProvider GetEntryPoint(IPlayer player, Assembly? preferredAssembly = null)
    {
        var playerStableHashCode = player.GetStableHashCode();
        return Combine(
        [
            .. _plugins.Keys.OrderByDescending(assembly => assembly == preferredAssembly).Select(assembly => GetPlayerScope(assembly, playerStableHashCode, () => player.Context)),
            GetEntryPoint(preferredAssembly)
        ]);
    }

    public IServiceProvider GetEntryPoint(Assembly? preferredAssembly = null)
    {
        var assemblies = _plugins.Keys.AsEnumerable();
        
        if (preferredAssembly is not null)
            assemblies = assemblies.OrderByDescending(assembly => assembly == preferredAssembly);
        
        var containers = assemblies.Select(assembly => GetPluginContainer(assembly).Root);
        return Combine([..containers, rootContainer]);
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

    private IServiceProvider GetPlayerScope(Assembly assembly, int playerStableHashCode, Func<IPlayerContext> getContext)
    {
        var container = GetPluginContainer(assembly);
        
        if (!container.Scopes.TryGetValue(playerStableHashCode, out var playerScope))
            container.Scopes[playerStableHashCode] = playerScope = GetPluginContainer(assembly).Root.GetRequiredService<IContainer>().OpenScope();

        playerScope.Add(ServiceDescriptor.Singleton(_ => getContext()));
        
        return playerScope;
    }
    
    private PluginContainer GetPluginContainer(Assembly pluginAssembly, CancellationToken cancellationToken = default)
    {
        if (_plugins.TryGetValue(pluginAssembly, out var pluginContainer))
            return pluginContainer;

        var emptyContainer = Combine();
        _plugins.Add(pluginAssembly, pluginContainer = new PluginContainer(Root: emptyContainer, Scopes: []));
        
        return pluginContainer;
    }
    
    private Container Combine(params IEnumerable<IServiceProvider> containers)
    {
        var events = rootContainer.Resolve<IEventService>();
        return new Container(rootContainer.Rules.WithUnknownServiceResolvers(request => ResolveFactory(request.ServiceType)));

        InstanceFactory? ResolveFactory(Type serviceType)
        {
            var service = ResolveService(serviceType);

            if (service is null)
                return null;

            return new InstanceFactory(service);
        }

        object? ResolveService(Type serviceType)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            foreach (var serviceProvider in containers)
            {
                var container = serviceProvider.GetRequiredService<IContainer>();

                if (!container.CanGetService(serviceType))
                    continue;

                var service = container
                    .With(dependencyRules => dependencyRules
                        .WithUnknownServiceResolvers(dependencyRequest => 
                            ResolveFactory(dependencyRequest.ServiceType)))
                    .GetService(serviceType);

                if (service is null)
                    continue;

                if (service is IEventListener listener)
                    events.RegisterListeners(listener);

                return service;
            }

            return null;
        }
    }
    
    private record PluginContainer(IServiceProvider Root, Dictionary<int, IResolverContext> Scopes);
}
