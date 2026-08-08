using Microsoft.Extensions.Logging;

namespace Void.Proxy.Api.Logging;

/// <summary>
/// Provides mutable control over the minimum log level used by the proxy logging pipeline.
/// </summary>
public interface ILogLevelSwitch
{
    /// <summary>
    /// Gets or sets the active minimum log level.
    /// </summary>
    public LogLevel Level { get; set; }
}
