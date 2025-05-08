using Microsoft.Extensions.Logging;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Plugins;
using Void.Proxy.Api.Plugins;
using Void.Proxy.Api.Plugins.Dependencies;

namespace Void.Proxy.Plugins.Watchdog;

public class Plugin(ILogger logger, IDependencyService dependencies) : IPlugin
{
    public string Name => nameof(Plugin);

    [Subscribe]
    public void OnPluginLoading(PluginLoadingEvent @event)
    {
        if (@event.Plugin != this)
            return;

        dependencies.Register(services =>
        {
        });
    }
}
