﻿using Void.Proxy.API.Network.IO.Channels;

namespace Void.Proxy.API.Players;

public interface IPlayerContext : IAsyncDisposable
{
    public IServiceProvider Services { get; }
    public IMinecraftChannel? Channel { get; set; }
}