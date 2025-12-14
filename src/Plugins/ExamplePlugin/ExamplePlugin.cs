using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Plugins;
using Void.Proxy.Api.Events.Proxy;
using Void.Proxy.Api.Plugins;
using Void.Proxy.Api.Plugins.Dependencies;
using Void.Proxy.Plugins.ExamplePlugin.Services;

namespace Void.Proxy.Plugins.ExamplePlugin;

// Implementing IPlugin makes the class an entry point to your plugin.
// Constructor arguments are used to inject any API services implemented by proxy and other plugins.
// See ../Services/ directory for more examples.
public class ExamplePlugin(ILogger logger, IDependencyService dependencies) : IPlugin
{
    public string Name => nameof(ExamplePlugin);

    [Subscribe]
    public void OnPluginLoading(PluginLoadingEvent @event)
    {
        // This event is fired when any plugin is being loaded

        // Skip all other plugin load events except our plugin
        if (@event.Plugin != this)
            return;

        dependencies.Register(services =>
        {
            // Your services will be exposed to other plugins
            services.AddSingleton<ChatService>();

            // Scoped services are instantiated per-player
            services.AddScoped<InventoryService>();
        });
    }

    [Subscribe]
    public void OnProxyStarting(ProxyStartingEvent @event)
    {
        logger.LogInformation("Received ProxyStarting event");
    }

    [Subscribe]
    public void OnProxyStopping(ProxyStoppingEvent @event)
    {
        logger.LogInformation("Received ProxyStopping event");
    }
}
