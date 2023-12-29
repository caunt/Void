using Void.Proxy.API.Network.IO;

namespace Void.Proxy.API.Events.Handshake;

public class SearchServerProtocolCodec : IEventWithResult<IMinecraftChannel>
{
    public required ILink Link { get; init; }
    public required ServerInfo Server { get; init; }
    public IMinecraftChannel? Result { get; set; }
}
