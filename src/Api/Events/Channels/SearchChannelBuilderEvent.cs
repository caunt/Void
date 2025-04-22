using Void.Proxy.Api.Network.Channels;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Api.Events.Channels;

public record SearchChannelBuilderEvent(IPlayer Player, Memory<byte> Buffer) : IScopedEventWithResult<ChannelBuilder>
{
    public ChannelBuilder? Result { get; set; }
}
