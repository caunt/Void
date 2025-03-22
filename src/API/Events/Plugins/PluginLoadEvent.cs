using Void.Proxy.API.Plugins;

namespace Void.Proxy.API.Events.Plugins;

public record PluginLoadEvent(IPlugin Plugin) : IEvent;