using Void.Proxy.API.Links;
using Void.Proxy.API.Network.IO.Channels;
using Void.Proxy.API.Players;
using Void.Proxy.API.Servers;

namespace Void.Proxy.API.Events.Links;

public class CreateLinkEvent : IEventWithResult<ILink>
{
    public required IPlayer Player { get; init; }
    public required IServer Server { get; init; }
    public required IMinecraftChannel PlayerChannel { get; init; }
    public required IMinecraftChannel ServerChannel { get; init; }
    public ILink? Result { get; set; }
}