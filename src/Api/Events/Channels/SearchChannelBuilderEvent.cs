using Void.Common.Events;
using Void.Common.Players;
using Void.Proxy.Api.Network.Channels;

namespace Void.Proxy.Api.Events.Channels;

public record SearchChannelBuilderEvent(IPlayer Player, Memory<byte> Buffer) : IEventWithResult<ChannelBuilder>
{
    public ChannelBuilder? Result { get; set; }
}
