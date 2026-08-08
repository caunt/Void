using Void.Proxy.Api.Plugins;

namespace Void.Proxy.Api.Events.Plugins;

/// <summary>
/// Signals that a plugin is about to be loaded.
/// </summary>
/// <param name="Plugin">The plugin entering the loading phase.</param>
public record PluginLoadingEvent(IPlugin Plugin) : IEvent;
