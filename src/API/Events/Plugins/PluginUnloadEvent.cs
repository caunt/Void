using Void.Proxy.API.Plugins;

namespace Void.Proxy.API.Events.Plugins;

public record PluginUnloadEvent(IPlugin Plugin) : IEvent;