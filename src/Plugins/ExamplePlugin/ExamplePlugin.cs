using Microsoft.Extensions.Logging;
using Void.Common.Plugins;
using Void.Proxy.Api.Configurations;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Plugins;
using Void.Proxy.Api.Events.Proxy;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Plugins.ExamplePlugin.Services;

namespace Void.Proxy.Plugins.ExamplePlugin;

public class ExamplePlugin(ILogger<ExamplePlugin> logger, IEventService events, IConfigurationService configs) : IPlugin
{
    public string Name => nameof(ExamplePlugin);

    [Subscribe]
    public void OnPluginLoad(PluginLoadEvent @event)
    {
        if (@event.Plugin != this)
            return;

        events.RegisterListener<InventoryService>();
        events.RegisterListener<ChatService>();
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
