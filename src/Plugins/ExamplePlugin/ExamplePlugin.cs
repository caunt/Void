using Microsoft.Extensions.Logging;
using Void.Common.Plugins;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Plugins;
using Void.Proxy.Api.Events.Proxy;
using Void.Proxy.Api.Plugins;

namespace Void.Proxy.Plugins.ExamplePlugin;

public class ExamplePlugin(ILogger<ExamplePlugin> logger, IDependencyService dependencies) : IPlugin
{
    public string Name => nameof(ExamplePlugin);

    [Subscribe]
    public void OnPluginLoad(PluginLoadEvent @event)
    {
        if (@event.Plugin != this)
            return;

        // dependencies.Register(services =>
        // {
        //     services.AddSingleton<InventoryService>();
        //     services.AddSingleton<ChatService>();
        // });
        // 
        // var test = dependencies.Services;
        // 
        // Debugger.Break();
        // test.GetService<InventoryService>();
        // ActivatorUtilities.CreateInstance<InventoryService>(test);
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
