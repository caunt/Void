﻿using Void.Common.Events;
using Void.Common.Players;
using Void.Proxy.Api.Servers;

namespace Void.Proxy.Api.Events.Player;

public record PlayerSearchServerEvent(IPlayer Player) : IEventWithResult<IServer>
{
    public IServer? Result { get; set; }
}
