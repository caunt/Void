﻿using Void.Proxy.Api.Links;
using Void.Proxy.Api.Network.Channels;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Servers;

namespace Void.Proxy.Api.Events.Links;

public record CreateLinkEvent(IPlayer Player, IServer Server, INetworkChannel PlayerChannel, INetworkChannel ServerChannel) : IScopedEventWithResult<ILink>
{
    public ILink? Result { get; set; }
}
