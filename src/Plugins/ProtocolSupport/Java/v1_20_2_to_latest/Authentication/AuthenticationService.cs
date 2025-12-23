using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Void.Minecraft.Links.Extensions;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Messages.Binary;
using Void.Minecraft.Network.Messages.Packets;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Authentication;
using Void.Proxy.Api.Events.Network;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Players.Extensions;
using Void.Proxy.Api.Plugins.Dependencies;
using Void.Proxy.Plugins.Common.Events;
using Void.Proxy.Plugins.Common.Extensions;
using Void.Proxy.Plugins.Common.Network.Bundles;
using Void.Proxy.Plugins.Common.Network.Packets.Clientbound;
using Void.Proxy.Plugins.Common.Network.Packets.Serverbound;
using Void.Proxy.Plugins.Common.Services.Authentication;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Extensions;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Clientbound;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Serverbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Authentication;

public class AuthenticationService(ILogger<AuthenticationService> logger, IEventService events, IPlayerService players, IDependencyService dependencies) : AbstractAuthenticationService(events, players, dependencies)
{
    [Subscribe]
    public static void OnMessageReceived(MessageReceivedEvent @event)
    {
        switch (@event.Message)
        {
            case IMinecraftBinaryMessage binaryMessage:
                if (@event.Direction is not Direction.Serverbound)
                    break;

                if (!@event.Player.IsMinecraft)
                    break;

                if (@event.Player.Phase is not Phase.Play)
                    break;

                // TODO is it safe to cancel Player Session (chat_session_update) packet?
                // helps to join vanilla servers (from 1.19.3?)
                // UPDATE: cancelling this packet causes chat to stop working
                // if (player.ProtocolVersion >= ProtocolVersion.MINECRAFT_1_21_2)
                // {
                //     if (binaryMessage.Id is 0x08)
                //         @event.Result = true;
                // }

                break;
        }
    }

    protected override bool IsSupportedVersion(ProtocolVersion protocolVersion)
    {
        return Plugin.SupportedVersions.Contains(protocolVersion);
    }

    protected override async ValueTask<bool> IsPlayerAuthenticatedAsync(IPlayer player, CancellationToken cancellationToken)
    {
        return await player.IsAuthenticatedAsync(cancellationToken);
    }

    protected override async ValueTask<bool> IsPlayerPlayingAsync(IPlayer player, CancellationToken cancellationToken)
    {
        return await player.IsPlayingAsync(cancellationToken);
    }

    protected override async ValueTask<int> ReceivePlayerHandshakeAsync(ILink link, CancellationToken cancellationToken)
    {
        var handshake = await link.ReceivePacketAsync<HandshakePacket>(cancellationToken);

        // Status query
        if (handshake.NextState is not 2 and not 3)
            await link.SendPacketAsync(handshake, cancellationToken);

        return handshake.NextState;
    }

    protected override async ValueTask FinishPlayingAsync(ILink link, CancellationToken cancellationToken)
    {
        // IPlayer might be in middle of Bundle when client is not handling packets until closing bundle received
        var bundles = link.Player.Context.Services.GetRequiredService<IBundleService>();

        if (bundles.IsActivated)
            await link.SendPacketAsync<BundleDelimiterPacket>(cancellationToken);

        // tell IPlayer client to switch to Configuration state
        await link.SendTerminalPacketAsync<StartConfigurationPacket>(cancellationToken);

        // IPlayer might be still sending Play state packets, read them all until configuration acknowledged
        var playPacketsLimit = 256;

        IMinecraftServerboundPacket packet;
        while ((packet = await link.ReceivePacketAsync<IMinecraftServerboundPacket>(cancellationToken)) is not AcknowledgeConfigurationPacket)
        {
            if (playPacketsLimit-- is 0)
                throw new Exception("Client expected to send acknowledge configuration packet in order to start authentication");

            logger.LogTrace("Skipped serverbound packet {Packet} because authentication service is waiting for configuration acknowledge", packet);
        }
    }

    protected override async ValueTask<bool> StartPlayerLoginAsync(ILink link, CancellationToken cancellationToken)
    {
        if (!link.Player.IsMinecraft)
            return false;

        var loginStart = await link.ReceivePacketAsync<LoginStartPacket>(cancellationToken);

        if (IsAlreadyOnline(loginStart.Profile.Username))
            return false;

        link.Player.Profile = loginStart.Profile;
        return true;
    }

    protected override async ValueTask FinishPlayerLoginAsync(ILink link, CancellationToken cancellationToken)
    {
        if (!link.Player.IsMinecraft)
            return;

        if (link.Player.Profile is not { } profile)
            throw new InvalidOperationException("Player should be logged in already");

        await link.SendPacketAsync(new LoginSuccessPacket
        {
            GameProfile = profile,
            StrictErrorHandling = false
        }, cancellationToken);

        await link.ReceivePacketAsync<LoginAcknowledgedPacket>(cancellationToken);
    }

    protected override async ValueTask HandshakeWithServerAsync(ILink link, IMinecraftServerboundPacket? packet, int nextState, CancellationToken cancellationToken)
    {
        if (!link.Player.IsMinecraft)
            return;

        await link.SendPacketAsync(packet ?? new HandshakePacket
        {
            NextState = nextState,
            ProtocolVersion = link.Player.ProtocolVersion.Version,
            ServerAddress = link.Server.Host,
            ServerPort = (ushort)link.Server.Port
        }, cancellationToken);
    }

    protected override async ValueTask StartServerLoginAsync(ILink link, CancellationToken cancellationToken)
    {
        if (!link.Player.IsMinecraft)
            return;

        if (link.Player.Profile is not { } profile)
            throw new InvalidOperationException("Player should be logged in already");

        await link.SendPacketAsync(new LoginStartPacket
        {
            Profile = profile
        }, cancellationToken);
    }

    protected override async ValueTask<AuthenticationResult> HandleServerPacketAsync(ILink link, IMinecraftClientboundPacket packet, CancellationToken cancellationToken)
    {
        switch (packet)
        {
            case LoginDisconnectPacket loginDisconnectPacket:
                logger.LogInformation("Player {Player} cannot authenticate on {Server}: {Reason}", link.Player, link.Server, loginDisconnectPacket.Reason.SerializeLegacy());

                // Since the IPlayer client has already completed the login state, it cannot be kicked with a login disconnect packet
                await link.Player.KickAsync(loginDisconnectPacket.Reason, cancellationToken);
                return AuthenticationResult.NotAuthenticatedServer;
            case LoginSuccessPacket:
                await link.SendPacketAsync<LoginAcknowledgedPacket>(cancellationToken);
                return AuthenticationResult.Authenticated;
            case SetCompressionPacket:
                // handled by compression service
                break;
            case LoginPluginRequestPacket loginPluginRequest:
                var loginPluginMessage = new LoginPluginMessageEvent(link.Player, link, loginPluginRequest.Channel, loginPluginRequest.Data);

                if (!await events.ThrowWithResultAsync(loginPluginMessage, cancellationToken))
                    break;

                await link.SendPacketAsync(new LoginPluginResponsePacket
                {
                    MessageId = loginPluginRequest.MessageId,
                    Data = loginPluginMessage.Response ?? [],
                    Successful = loginPluginMessage.Successful
                }, cancellationToken);
                break;
            case EncryptionRequestPacket:
                throw new InvalidOperationException("Authentication side is set to Proxy, but server is in online-mode.");
            default:
                throw new InvalidOperationException($"Unexpected {packet} packet received");
        }

        return AuthenticationResult.NoResult;
    }
}
