using System.Diagnostics.CodeAnalysis;
using Void.Proxy.API.Players;
using Void.Proxy.API.Servers;

namespace Void.Proxy.API.Links;

public interface ILinkService
{
    public ValueTask<ConnectionResult> ConnectPlayerAnywhereAsync(IPlayer player, CancellationToken cancellationToken = default);
    public ValueTask<ConnectionResult> ConnectAsync(IPlayer player, IServer server, CancellationToken cancellationToken = default);
    public bool TryGetLink(IPlayer player, [NotNullWhen(true)] out ILink? link);
}