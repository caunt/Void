using Void.Proxy.API.Network.IO.Channels.Services;
using Void.Proxy.API.Players;

namespace Void.Proxy.API.Events.Channels;

public record SearchChannelBuilderEvent(IPlayer Player, Memory<byte> Buffer) : IEventWithResult<ChannelBuilder>
{
    public ChannelBuilder? Result { get; set; }
}