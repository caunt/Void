using System.Diagnostics.CodeAnalysis;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Servers;

namespace Void.Proxy.Api.Links;

public interface ILinkService
{
    public IReadOnlyList<ILink> All { get; }

    public ValueTask<ConnectionResult> ConnectPlayerAnywhereAsync(IPlayer player, CancellationToken cancellationToken = default);
    public ValueTask<ConnectionResult> ConnectPlayerAnywhereAsync(IPlayer player, IEnumerable<IServer> ignoredServers, CancellationToken cancellationToken = default);
    public ValueTask<ConnectionResult> ConnectAsync(IPlayer player, IServer server, CancellationToken cancellationToken = default);
    public bool TryGetLink(IPlayer player, [NotNullWhen(true)] out ILink? link);
    public bool TryGetWeakLink(IPlayer player, [NotNullWhen(true)] out ILink? link);
    public bool HasLink(IPlayer player);
}
