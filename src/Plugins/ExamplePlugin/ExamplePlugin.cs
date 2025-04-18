using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Plugins;
using Void.Proxy.Api.Events.Proxy;
using Void.Proxy.Api.Plugins;
using Void.Proxy.Api.Plugins.Dependencies;
using Void.Proxy.Plugins.ExamplePlugin.Services;

namespace Void.Proxy.Plugins.ExamplePlugin;

// Implementing IPlugin makes class an entry point to your plugin
// Constructor arguments are used to inject many API services implemented by proxy and other plugins
// See ../Services/ directory for more examples
public class ExamplePlugin(ILogger<ExamplePlugin> logger, IDependencyService dependencies) : IPlugin
{
    public string Name => nameof(ExamplePlugin);

    [Subscribe]
    public void OnPluginLoad(PluginLoadEvent @event)
    {
        // This event is fired when any plugin is being loaded

        // Skip all other plugins load events except ours
        if (@event.Plugin != this)
            return;

        dependencies.Register(services =>
        {
            // You can expose your services to other plugins
            services.AddSingleton<InventoryService>();
            services.AddSingleton<ChatService>();
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
