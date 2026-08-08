namespace Void.Proxy.Api.Network.Exceptions;

/// <summary>
/// The exception thrown when a network stream operation exceeds its permitted duration.
/// </summary>
/// <param name="operation">The operation that timed out.</param>
public class StreamTimeoutException(Operation operation) : StreamException
{
    /// <summary>
    /// Gets the operation that timed out.
    /// </summary>
    public Operation Operation => operation;
}
