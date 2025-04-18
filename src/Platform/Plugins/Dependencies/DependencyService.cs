using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Plugins;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Extensions;
using Void.Proxy.Api.Plugins;
using Void.Proxy.Api.Plugins.Dependencies;

namespace Void.Proxy.Plugins.Dependencies;

public class DependencyService(ILogger<DependencyService> logger, IServiceProvider services, IEventService events) : IDependencyService
{
    [Subscribe(PostOrder.First)]
    public void OnPluginUnloading(PluginUnloadingEvent @event)
    {
        var assembly = @event.Plugin.GetType().Assembly;

        services.Remove(descriptor =>
        {
            var instance = descriptor.CreateInstance(services);
            return instance?.GetType().Assembly == assembly;
        });
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
        var instance = services.HasService(serviceType) ? services.GetRequiredService(serviceType) : ActivatorUtilities.CreateInstance(services, serviceType);

        if (instance is IEventListener listener)
            events.RegisterListeners(listener);

        if (instance is IPlugin plugin)
            RegisterPlugin(plugin);

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
        return provider(services);
    }

    public void Register(Action<ServiceCollection> configure, bool activate = true)
    {
        var configuredServices = new ServiceCollection();
        configure(configuredServices);
        configuredServices.RegisterListeners();

        foreach (var service in configuredServices)
        {
            services.Add(service);
        }


        if (!activate)
            return;

        foreach (var descriptor in services.GetDescriptors().Where(descriptor => descriptor.Lifetime is ServiceLifetime.Singleton && !descriptor.ServiceType.ContainsGenericParameters))
        {
            var instance = services.GetService(descriptor.ServiceType);

            if (instance is IEventListener listener)
                events.RegisterListeners(listener);
        }
    }

    private void RegisterPlugin(IPlugin plugin)
    {
        var pluginType = plugin.GetType();

        logger.LogTrace("Injecting {Plugin} into {Name} service collection", plugin.GetType(), plugin.Name);

        // Plugin => Plugin
        services.Add(ServiceDescriptor.Singleton(pluginType, plugin));

        services.GetRequiredService(pluginType);
    }
}
