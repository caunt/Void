using Void.Common.Events;
using Void.Common.Plugins;

namespace Void.Proxy.Api.Events.Plugins;

public record PluginLoadEvent(IPlugin Plugin) : IEvent;
