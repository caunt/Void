using System.Text.Json.Nodes;
using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Player;
using Void.Proxy.API.Links;
using Void.Proxy.API.Mojang.Minecraft.Network;
using Void.Proxy.API.Players;
using Void.Proxy.Plugins.Common.Events;

namespace Void.Proxy.Plugins.Common.Services.Lifecycle;

public abstract class AbstractLifecycleService : IPluginService
{
    [Subscribe(PostOrder.Last)]
    public async ValueTask OnPlayerKickEvent(PlayerKickEvent @event, CancellationToken cancellationToken)
    {
        if (!IsSupportedVersion(@event.Player.ProtocolVersion))
            return;

        var reason = string.IsNullOrWhiteSpace(@event.Reason) ? "You were kicked from proxy" : @event.Reason;

        if (@event.Player.ProtocolVersion < ProtocolVersion.MINECRAFT_1_20_3)
        {
            try
            {
                JsonNode.Parse(reason);
            }
            catch
            {
                reason = $"{{\"text\":\"{reason}\"}}";
            }
        }

        @event.Result = await KickPlayerAsync(@event.Player, reason, cancellationToken);
    }

    [Subscribe]
    public async ValueTask OnPlayerVerifiedEncryption(PlayerVerifiedEncryptionEvent @event, CancellationToken cancellationToken)
    {
        if (!IsSupportedVersion(@event.Link.Player.ProtocolVersion))
            return;

        await EnableCompressionAsync(@event.Link, cancellationToken);
    }

    protected abstract ValueTask EnableCompressionAsync(ILink link, CancellationToken cancellationToken);
    protected abstract ValueTask<bool> KickPlayerAsync(IPlayer player, string reason, CancellationToken cancellationToken);
    protected abstract bool IsSupportedVersion(ProtocolVersion version);
}
