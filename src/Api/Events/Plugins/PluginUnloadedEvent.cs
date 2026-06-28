using Void.Proxy.Api.Plugins;

namespace Void.Proxy.Api.Events.Plugins;

/// <summary>
/// Represents the notification raised for a plugin after its unloading event has been fired.
/// </summary>
/// <param name="Plugin">The <see cref="IPlugin" /> instance being reported as unloaded.</param>
public record PluginUnloadedEvent(IPlugin Plugin) : IEvent;
