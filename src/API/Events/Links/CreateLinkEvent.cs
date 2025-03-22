using Void.Proxy.API.Links;
using Void.Proxy.API.Network.IO.Channels;
using Void.Proxy.API.Players;
using Void.Proxy.API.Servers;

namespace Void.Proxy.API.Events.Links;

public record CreateLinkEvent(IPlayer Player, IServer Server, IMinecraftChannel PlayerChannel, IMinecraftChannel ServerChannel) : IEventWithResult<ILink>
{
    public ILink? Result { get; set; }
}