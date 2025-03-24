using System.Diagnostics.CodeAnalysis;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Servers;

namespace Void.Proxy.Api.Links;

public interface ILinkService
{
    public ValueTask<ConnectionResult> ConnectPlayerAnywhereAsync(IPlayer player, CancellationToken cancellationToken = default);
    public ValueTask<ConnectionResult> ConnectAsync(IPlayer player, IServer server, CancellationToken cancellationToken = default);
    public bool TryGetLink(IPlayer player, [NotNullWhen(true)] out ILink? link);
}