﻿using Void.Common.Players;

namespace Void.Proxy.Api.Chat.Commands;

public interface IPlayerCommandSource
{
    public IPlayer Player { get; set; }
}
