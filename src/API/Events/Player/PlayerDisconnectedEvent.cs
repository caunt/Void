﻿using Void.Proxy.API.Players;

namespace Void.Proxy.API.Events.Player;

public record PlayerDisconnectedEvent(IPlayer Player) : IEvent;