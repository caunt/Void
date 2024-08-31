using Void.Proxy.API.Plugins;

namespace Void.Proxy.API.Events.Plugins;

public class PluginUnloadEvent : IEvent
{
    public required IPlugin Plugin { get; init; }
}