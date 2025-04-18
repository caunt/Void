using Void.Proxy.Api.Plugins;

namespace Void.Proxy.Api.Events.Plugins;

public record PluginUnloadEvent(IPlugin Plugin) : IEvent;
