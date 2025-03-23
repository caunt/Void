using Void.Proxy.API.Network.IO.Channels;
using Void.Proxy.API.Players;

namespace Void.Proxy.Players.Contexts;

public class EmptyPlayerContext : IPlayerContext
{
    public static readonly EmptyPlayerContext Instance = new();

    public IServiceProvider Services => throw new NotImplementedException();

    public IMinecraftChannel? Channel { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
}
