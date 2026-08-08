using Void.Proxy.Api.Plugins;

namespace Void.Proxy.Api.Events.Plugins;

/// <summary>
/// Signals that a plugin has finished loading.
/// </summary>
/// <param name="Plugin">The plugin that finished loading.</param>
public record PluginLoadedEvent(IPlugin Plugin) : IEvent;
