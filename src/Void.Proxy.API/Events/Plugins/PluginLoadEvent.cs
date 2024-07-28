using Void.Proxy.API.Plugins;

namespace Void.Proxy.API.Events.Plugins;

public class PluginLoadEvent : IEvent
{
    public required IPlugin Plugin { get; init; }
}