using Void.Common.Events;
using Void.Common.Network.Channels;
using Void.Common.Players;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Servers;

namespace Void.Proxy.Api.Events.Links;

public record CreateLinkEvent(IPlayer Player, IServer Server, INetworkChannel PlayerChannel, INetworkChannel ServerChannel) : IEventWithResult<ILink>
{
    public ILink? Result { get; set; }
}
