﻿using Void.Proxy.Api.Network.IO.Channels;
using Void.Proxy.Api.Players;

namespace Void.Proxy.Players.Contexts;

public class PlayerContext(IServiceProvider services) : IPlayerContext
{
    public IServiceProvider Services => services;

    public IMinecraftChannel? Channel { get; set; }

    public async ValueTask DisposeAsync()
    {
        if (Channel is not null)
            await Channel.DisposeAsync();

        GC.SuppressFinalize(this);
    }
}