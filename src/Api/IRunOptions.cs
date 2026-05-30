namespace Void.Proxy.Api;

/// <summary>
/// Provides immutable startup options used to configure a proxy run before the host and services are created.
/// </summary>
/// <remarks>
/// <para>
/// Implementations are registered as a singleton service and are consumed by startup, console parsing, plugin loading,
/// configuration loading, and dependency resolution code. The values are treated as run-scoped inputs rather than live
/// mutable settings.
/// </para>
/// <para>
/// <see cref="WorkingDirectory"/> is used as the base path for runtime files and plugin dependency probing.
/// <see cref="Arguments"/> is parsed by the console command infrastructure to discover explicitly supplied command-line
/// options such as listener, logging, or offline-mode overrides.
/// </para>
/// </remarks>
/// <seealso cref="IProxy"/>
public interface IRunOptions
{
    public string WorkingDirectory { get; }
    public string[] Arguments { get; }
}
