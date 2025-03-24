using Void.Proxy.Api.Servers;

namespace Void.Proxy.Api.Events.Encryption;

public record SearchServerPrivateKey(IServer Server) : IEventWithResult<byte[]>
{
    public byte[]? Result { get; set; }
}