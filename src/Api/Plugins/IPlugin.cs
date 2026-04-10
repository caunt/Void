using Void.Proxy.Api.Events;

namespace Void.Proxy.Api.Plugins;

/// <summary>
/// Represents a proxy plugin. Plugins extend proxy functionality by subscribing to events
/// and are identified by a human-readable name.
/// </summary>
public interface IPlugin : IEventListener
{
    /// <summary>
    /// Gets the human-readable name of this plugin.
    /// </summary>
    /// <value>A short, descriptive label used in log output and diagnostic messages.</value>
    public string Name { get; }
}
