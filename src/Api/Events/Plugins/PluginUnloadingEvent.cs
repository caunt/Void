using Void.Proxy.Api.Plugins;

namespace Void.Proxy.Api.Events.Plugins;

public record PluginUnloadingEvent(IPlugin Plugin) : IEvent;
