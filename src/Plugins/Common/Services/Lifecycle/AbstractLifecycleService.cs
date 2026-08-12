using Microsoft.Extensions.Logging;
using Void.Minecraft.Components.Text;
using Void.Minecraft.Events;
using Void.Minecraft.Events.Chat;
using Void.Minecraft.Links.Extensions;
using Void.Minecraft.Network;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Links;
using Void.Proxy.Api.Events.Network;
using Void.Proxy.Api.Events.Player;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Network.Exceptions;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Players.Extensions;
using Void.Proxy.Plugins.Common.Events;
using Void.Proxy.Plugins.Common.Network.Packets.Clientbound;
using Void.Proxy.Plugins.Common.Network.Packets.Serverbound;
using Void.Proxy.Plugins.Common.Players;
using Void.Proxy.Plugins.Common.Players.Contexts;

namespace Void.Proxy.Plugins.Common.Services.Lifecycle;

public abstract class AbstractLifecycleService(ILogger logger, IEventService events) : IPluginCommonService
{
    private const string DefaultKickMessage = "You were kicked from proxy";

    private readonly TimeSpan _keepAliveInterval = TimeSpan.FromSeconds(5);
    private readonly KeepAliveTrackerRegistry<IPlayer> _keepAliveTrackers = new();

    [Subscribe]
    public void OnPlayerConnecting(PlayerConnectingEvent @event)
    {
        @event.Result ??= new SimplePlayer(@event.Client, instance => new PlayerContext(@event.GetServices) { Player = instance });
        logger.LogTrace("Player connecting: {Player}", @event.Result);
    }

    [Subscribe]
    public async ValueTask OnPlayerDisconnected(PlayerDisconnectedEvent @event)
    {
        if (!@event.Player.IsMinecraft)
            return;

        if (!IsSupportedVersion(@event.Player.ProtocolVersion))
            return;

        if (_keepAliveTrackers.Remove(@event.Player) is { } tracker)
            await tracker.DisposeAsync();
    }

    [Subscribe]
    public async ValueTask OnPhaseChanged(PhaseChangedEvent @event, CancellationToken cancellationToken)
    {
        if (!IsSupportedVersion(@event.Player.ProtocolVersion))
            return;

        if (@event.Phase is Phase.Configuration or Phase.Play && @event.Link is not null)
            GetOrCreateKeepAliveTracker(@event.Player);

        if (@event.Phase is Phase.Play && @event.Side is Side.Server && @event.Player.ProtocolVersion >= ProtocolVersion.MINECRAFT_1_20_2)
        {
            var link = @event.Player.Link ?? throw new InvalidOperationException($"Player has no link assigned in {nameof(Phase.Play)} phase.");
            await events.ThrowAsync(new PlayerJoinedServerEvent(link.Player, link.Server, link, IsRedirected: false), cancellationToken);
        }
    }

    [Subscribe]
    public async ValueTask OnLinkStarted(LinkStartedEvent @event, CancellationToken cancellationToken)
    {
        if (!@event.Player.IsMinecraft)
            return;

        if (!IsSupportedVersion(@event.Player.ProtocolVersion))
            return;

        if (@event.Player.ProtocolVersion >= ProtocolVersion.MINECRAFT_1_20_2)
            return;

        await events.ThrowAsync(new PlayerJoinedServerEvent(@event.Link.Player, @event.Link.Server, @event.Link, IsRedirected: !@event.IsFirstLink), cancellationToken);
    }

    [Subscribe]
    public async ValueTask OnMessageReceived(MessageReceivedEvent @event, CancellationToken cancellationToken)
    {
        if (!@event.Player.IsMinecraft)
            return;

        if (!IsSupportedVersion(@event.Player.ProtocolVersion))
            return;

        switch (@event.Message)
        {
            case KeepAliveRequestPacket keepAliveRequest:
                @event.Cancel();
                await @event.Link.SendPacketAsync(new KeepAliveResponsePacket { Id = keepAliveRequest.Id }, cancellationToken);
                break;
            case KeepAliveResponsePacket keepAliveResponse:
                if (KeepAliveTracker.IsLegacyTerrainKeepAlive(@event.Player.ProtocolVersion, keepAliveResponse.Id))
                    break;

                @event.Cancel();

                if (_keepAliveTrackers.Get(@event.Player) is { } tracker)
                    tracker.Pong(keepAliveResponse.Id, cancellationToken);

                break;
        }
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
            ? minecraftPlayerKick.Reason ?? DefaultKickMessage
            : @event.Text ?? DefaultKickMessage;

        try
        {
            @event.Result = await KickPlayerAsync(@event.Player, reason, cancellationToken);
        }
        catch (StreamException)
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

    private KeepAliveTracker GetOrCreateKeepAliveTracker(IPlayer player)
    {
        return _keepAliveTrackers.GetOrAdd(player, () => new KeepAliveTracker(player.Logger, async (requestId, cancellationToken) =>
        {
            player.Logger.LogTrace("Sending Keep Alive request {Id}", requestId);

            try
            {
                await player.SendPacketAsync(new KeepAliveRequestPacket { Id = requestId }, cancellationToken);
            }
            catch (StreamClosedException)
            {
                // Ignored
            }
            catch (Exception exception)
            {
                player.Logger.LogError(exception, "Failed to send Keep Alive request {Id}", requestId);
            }
        }, async (tracker, cancellationToken) =>
        {
            if (!_keepAliveTrackers.IsCurrent(player, tracker) || player.Context.Channel is not { IsAlive: true })
                return;

            try
            {
                player.Logger.LogInformation("Keep alive timed out");
                await player.KickAsync("Timed out", cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
            }
            catch (Exception exception)
            {
                player.Logger.LogError(exception, "Error while handling keep alive timeout");
            }
        }, _keepAliveInterval, createRequestIdFunction: () => KeepAliveTracker.CreateRequestId(player.ProtocolVersion)));
    }
}
