using Void.Proxy.Api.Servers;

namespace Void.Proxy.Api.Events.Encryption;

public record SearchServerPrivateKeyEvent(IServer Server) : IEventWithResult<byte[]>
{
    public byte[]? Result { get; set; }
}
