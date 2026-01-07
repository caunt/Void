namespace Void.Proxy.Api.Network.Exceptions;

public class StreamTimeoutException(Operation operation) : StreamException
{
    public Operation Operation => operation;
}
