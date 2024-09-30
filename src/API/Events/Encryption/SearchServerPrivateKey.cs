using Void.Proxy.API.Servers;

namespace Void.Proxy.API.Events.Encryption;

public class SearchServerPrivateKey : IEventWithResult<byte[]>
{
    public required IServer Server { get; init; }
    public byte[]? Result { get; set; }
}