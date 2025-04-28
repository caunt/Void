using Microsoft.Extensions.Logging;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Plugins;
using Void.Proxy.Api.Plugins;
using Void.Proxy.Api.Plugins.Dependencies;

namespace YourPlugin;

public class YourPlugin(ILogger logger, IDependencyService dependencies) : IPlugin
{
    public string Name => nameof(YourPlugin);

    [Subscribe]
    public void OnPluginLoading(PluginLoadingEvent @event)
    {
        if (@event.Plugin != this)
            return;

        logger.LogInformation("Registering {PluginName} services", Name);

        dependencies.Register(services =>
        {
            // Register your services here
            // See the ExamplePlugin source code to learn how to create your plugin:
            // https://github.com/caunt/Void/blob/main/src/Plugins/ExamplePlugin/ExamplePlugin.cs
            // https://github.com/caunt/Void/blob/main/src/Plugins/ExamplePlugin/Services/
        });
    }
}
