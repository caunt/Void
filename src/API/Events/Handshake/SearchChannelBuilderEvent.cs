using Void.Proxy.API.Network.IO.Channels.Services;
using Void.Proxy.API.Players;

namespace Void.Proxy.API.Events.Handshake;

public class SearchChannelBuilderEvent : IEventWithResult<ChannelBuilder>
{
    public required Memory<byte> Buffer { get; init; }
    public required IPlayer Player { get; init; }
    public ChannelBuilder? Result { get; set; }
}