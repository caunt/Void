namespace Void.Proxy.Api.Network;

/// <summary>
/// Specifies the stream operations to which a component or failure applies.
/// </summary>
[Flags]
public enum Operation
{
    /// <summary>
    /// Reads data from the channel.
    /// </summary>
    Read = 1,

    /// <summary>
    /// Writes data to the channel.
    /// </summary>
    Write = 2,

    /// <summary>
    /// Applies to both read and write operations.
    /// </summary>
    Any = Read | Write
}
