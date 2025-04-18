using Void.Minecraft.Components.Text;
using Void.Minecraft.Events;
using Void.Minecraft.Events.Chat;
using Void.Minecraft.Network;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Player;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Players;
using Void.Proxy.Plugins.Common.Events;

namespace Void.Proxy.Plugins.Common.Services.Lifecycle;

public abstract class AbstractLifecycleService : IPluginCommonService
{
    [Subscribe]
    public async ValueTask OnChatMessageSend(ChatMessageSendEvent @event, CancellationToken cancellationToken)
    {
        if (!@event.Player.TryGetMinecraftPlayer(out var player))
            return;

        if (!IsSupportedVersion(player.ProtocolVersion))
            return;

        await SendChatMessageAsync(@event.Player, @event.Text, cancellationToken);
    }

    [Subscribe(PostOrder.Last)]
    public async ValueTask OnPlayerKickEvent(PlayerKickEvent @event, CancellationToken cancellationToken)
    {
        if (!@event.Player.TryGetMinecraftPlayer(out var player))
            return;

        if (!IsSupportedVersion(player.ProtocolVersion))
            return;

        var reason = @event is MinecraftPlayerKickEvent minecraftPlayerKick
            ? minecraftPlayerKick.Reason is null
                ? "You were kicked from proxy"
                : minecraftPlayerKick.Reason
            : @event.Text is null
                ? "You were kicked from proxy"
                : @event.Text;

        @event.Result = await KickPlayerAsync(@event.Player, reason, cancellationToken);
    }

    [Subscribe]
    public async ValueTask OnPlayerVerifiedEncryption(PlayerVerifiedEncryptionEvent @event, CancellationToken cancellationToken)
    {
        if (!@event.Link.Player.TryGetMinecraftPlayer(out var player))
            return;

        if (!IsSupportedVersion(player.ProtocolVersion))
            return;

        await EnableCompressionAsync(@event.Link, cancellationToken);
    }

    protected abstract ValueTask EnableCompressionAsync(ILink link, CancellationToken cancellationToken);
    protected abstract ValueTask<bool> KickPlayerAsync(IPlayer player, Component reason, CancellationToken cancellationToken);
    protected abstract ValueTask<bool> SendChatMessageAsync(IPlayer player, Component text, CancellationToken cancellationToken);
    protected abstract bool IsSupportedVersion(ProtocolVersion version);
}
