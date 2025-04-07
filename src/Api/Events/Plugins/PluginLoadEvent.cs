using Void.Common;

namespace Void.Proxy.Api.Events.Plugins;

public record PluginLoadEvent(IPlugin Plugin) : IEvent;
