using Void.Common;

namespace Void.Proxy.Api.Events.Plugins;

public record PluginUnloadEvent(IPlugin Plugin) : IEvent;
