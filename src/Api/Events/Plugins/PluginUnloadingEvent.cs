using Void.Proxy.Api.Plugins;

namespace Void.Proxy.Api.Events.Plugins;

/// <summary>
/// Signals that a plugin is about to be unloaded.
/// </summary>
/// <param name="Plugin">The plugin entering the unloading phase.</param>
public record PluginUnloadingEvent(IPlugin Plugin) : IEvent;
