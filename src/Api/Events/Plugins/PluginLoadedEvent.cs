using Void.Proxy.Api.Plugins;

namespace Void.Proxy.Api.Events.Plugins;

public record PluginLoadedEvent(IPlugin Plugin) : IEvent;
