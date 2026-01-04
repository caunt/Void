using Void.Minecraft.Components.Text;
using Void.Minecraft.Events;
using Void.Minecraft.Events.Chat;
using Void.Minecraft.Network;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Links;
using Void.Proxy.Api.Events.Player;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Network.Exceptions;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Players.Extensions;
using Void.Proxy.Plugins.Common.Events;
using Void.Proxy.Plugins.Common.Players;
using Void.Proxy.Plugins.Common.Players.Contexts;

namespace Void.Proxy.Plugins.Common.Services.Lifecycle;

public abstract class AbstractLifecycleService(IEventService events) : IPluginCommonService
{
    [Subscribe]
    public static void OnPlayerConnecting(PlayerConnectingEvent @event)
    {
        @event.Result ??= new SimplePlayer(@event.Client, instance => new PlayerContext(@event.GetServices(instance)) { Player = instance });
    }

    [Subscribe]
    public async ValueTask OnPhaseChanged(PhaseChangedEvent @event)
    {
        if (!IsSupportedVersion(@event.Player.ProtocolVersion))
            return;

        if (@event.Phase is not Phase.Play)
            return;

        if (@event.Player.ProtocolVersion < ProtocolVersion.MINECRAFT_1_20_2)
            return;

        if (@event.Side is not Side.Server)
            return;

        var link = @event.Player.Link ?? throw new InvalidOperationException($"Player has no link assigned in {nameof(Phase.Play)} phase.");
        await events.ThrowAsync(new PlayerJoinedServerEvent(link.Player, link.Server, link));
    }

    [Subscribe]
    public async ValueTask OnLinkStarted(LinkStartedEvent @event)
    {
        if (!IsSupportedVersion(@event.Player.ProtocolVersion))
            return;

        if (@event.Player.ProtocolVersion >= ProtocolVersion.MINECRAFT_1_20_2)
            return;

        await events.ThrowAsync(new PlayerJoinedServerEvent(@event.Link.Player, @event.Link.Server, @event.Link));
    }

    [Subscribe]
    public async ValueTask OnChatMessageSend(ChatMessageSendEvent @event, CancellationToken cancellationToken)
    {
        if (@event.Origin is not Side.Proxy)
            return;

        if (!@event.Player.IsMinecraft)
            return;

        if (!IsSupportedVersion(@event.Player.ProtocolVersion))
            return;

        await SendChatMessageAsync(@event.Player, @event.Text, cancellationToken);
    }

    [Subscribe(PostOrder.Last)]
    public async ValueTask OnPlayerKickEvent(PlayerKickEvent @event, CancellationToken cancellationToken)
    {
        if (!@event.Player.IsMinecraft)
            return;

        if (!IsSupportedVersion(@event.Player.ProtocolVersion))
            return;

        var reason = @event is MinecraftPlayerKickEvent minecraftPlayerKick
            ? minecraftPlayerKick.Reason is null
                ? "You were kicked from proxy"
                : minecraftPlayerKick.Reason
            : @event.Text is null
                ? "You were kicked from proxy"
                : @event.Text;

        try
        {
            @event.Result = await KickPlayerAsync(@event.Player, reason, cancellationToken);
        }
        catch (StreamClosedException)
        {
            @event.Result = true;
        }
    }

    [Subscribe]
    public async ValueTask OnPlayerVerifiedEncryption(PlayerVerifiedEncryptionEvent @event, CancellationToken cancellationToken)
    {
        if (!@event.Player.IsMinecraft)
            return;

        if (!IsSupportedVersion(@event.Player.ProtocolVersion))
            return;

        await EnableCompressionAsync(@event.Link, cancellationToken);
    }

    protected abstract ValueTask EnableCompressionAsync(ILink link, CancellationToken cancellationToken);
    protected abstract ValueTask<bool> KickPlayerAsync(IPlayer player, Component reason, CancellationToken cancellationToken);
    protected abstract ValueTask<bool> SendChatMessageAsync(IPlayer player, Component text, CancellationToken cancellationToken);
    protected abstract bool IsSupportedVersion(ProtocolVersion version);
}
