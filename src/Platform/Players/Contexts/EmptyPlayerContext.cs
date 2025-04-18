﻿using Void.Proxy.Api.Network.Channels;
using Void.Proxy.Api.Players.Contexts;

namespace Void.Proxy.Players.Contexts;

public record EmptyPlayerContext : IPlayerContext
{
    public static readonly EmptyPlayerContext Instance = new();

    public IServiceProvider Services => throw new NotSupportedException();

    public INetworkChannel? Channel { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
}
