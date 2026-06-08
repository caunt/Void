namespace Void.Proxy.Api.Network;

[Flags]
public enum Operation
{
    /// <summary>
    /// Reads data from the channel.
    /// </summary>
    Read = 1,
    Write = 2,
    Any = Read | Write
}
