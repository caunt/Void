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
    public void OnProxyStart(ProxyStart @event)
    {
        logger.LogInformation("Received ProxyStart event");
    }

    [Subscribe]
    public void OnProxyStop(ProxyStop @event)
    {
        logger.LogInformation("Received ProxyStop event");
    }
}