using Void.Proxy.API.Servers;

namespace Void.Proxy.API.Events.Encryption;

public record SearchServerPrivateKey(IServer Server) : IEventWithResult<byte[]>
{
    public byte[]? Result { get; set; }
}