using Void.Proxy.Api.Links;
using Void.Proxy.Api.Network.IO.Channels;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Servers;

namespace Void.Proxy.Api.Events.Links;

public record CreateLinkEvent(IPlayer Player, IServer Server, IMinecraftChannel PlayerChannel, IMinecraftChannel ServerChannel) : IEventWithResult<ILink>
{
    public ILink? Result { get; set; }
}