using Void.Proxy.Api.Network.IO.Channels.Services;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Api.Events.Channels;

public record SearchChannelBuilderEvent(IPlayer Player, Memory<byte> Buffer) : IEventWithResult<ChannelBuilder>
{
    public ChannelBuilder? Result { get; set; }
}
