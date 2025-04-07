using Void.Common.Events;
using Void.Common.Plugins;

namespace Void.Proxy.Api.Events.Plugins;

public record PluginUnloadEvent(IPlugin Plugin) : IEvent;
