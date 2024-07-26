using Microsoft.Extensions.Logging;
using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Proxy;
using Void.Proxy.API.Plugins;

namespace Void.Proxy.Plugins.ExamplePlugin;

public class ExamplePlugin(ILogger<ExamplePlugin> logger) : IPlugin
{
    public string Name => nameof(ExamplePlugin);

    public Task ExecuteAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
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