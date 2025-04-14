using System;
using Void.Common.Network.Channels;

namespace Void.Common.Players;

public interface IPlayerContext : IAsyncDisposable
{
    public IServiceProvider Services { get; }
    public INetworkChannel? Channel { get; set; }
}
