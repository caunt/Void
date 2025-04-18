using Void.Common.Events;
using Void.Proxy.Api.Plugins;

namespace Void.Proxy.Api.Events.Plugins;

public record PluginLoadEvent(IPlugin Plugin) : IEvent;
