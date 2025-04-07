using Microsoft.Extensions.Logging;
using Void.Minecraft.Network;
using Void.Proxy.Api.Events.Authentication;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Links.Extensions;
using Void.Proxy.Api.Network.IO.Messages.Packets;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Players.Extensions;
using Void.Proxy.Plugins.Common.Events;
using Void.Proxy.Plugins.Common.Extensions;
using Void.Proxy.Plugins.Common.Services.Authentication;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Extensions;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Packets.Clientbound;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Packets.Serverbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Authentication;

public class AuthenticationService(ILogger<AuthenticationService> logger, IEventService events, IPlayerService players) : AbstractAuthenticationService(events, players)
{
    private readonly IEventService _events = events;

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

    protected override async ValueTask<int> GetHandshakeNextStateAsync(ILink link, CancellationToken cancellationToken)
    {
        var handshake = await link.ReceivePacketAsync<HandshakePacket>(cancellationToken);

        // Status query
        if (handshake.NextState is not 2 and not 3)
            await link.SendPacketAsync(handshake, cancellationToken);

        return handshake.NextState;
    }

    protected override async ValueTask<bool> IdentifyPlayerAsync(ILink link, CancellationToken cancellationToken)
    {
        var loginStart = await link.ReceivePacketAsync<LoginStartPacket>(cancellationToken);

        if (IsAlreadyOnline(loginStart.Profile.Username))
            return false;

        link.Player.Profile = loginStart.Profile;
        return true;
    }

    protected override async ValueTask AdmitPlayerAsync(ILink link, CancellationToken cancellationToken)
    {
        if (link.Player.Profile is null)
            throw new InvalidOperationException("Player should be identified before admitting");

        await link.SendPacketAsync(new LoginSuccessPacket
        {
            GameProfile = link.Player.Profile
        }, cancellationToken);
    }

    protected override async ValueTask PrepareServerAuthenticationAsync(ILink link, CancellationToken cancellationToken)
    {
        if (link.Player.Profile is null)
            throw new InvalidOperationException("Player should be admitted before preparing server");

        await link.SendPacketAsync(new HandshakePacket
        {
            NextState = 2,
            ProtocolVersion = link.Player.ProtocolVersion.Version,
            ServerAddress = link.Server.Host,
            ServerPort = (ushort)link.Server.Port
        }, cancellationToken);

        await link.SendPacketAsync(new LoginStartPacket
        {
            Profile = link.Player.Profile
        }, cancellationToken);
    }

    protected override async ValueTask FinishServerAuthenticationAsync(ILink link, AuthenticationResult authenticationResult, CancellationToken cancellationToken)
    {
        var joinGamePacket = await link.ReceivePacketAsync<JoinGamePacket>(cancellationToken);

        if (authenticationResult is AuthenticationResult.AlreadyAuthenticated)
            await link.SendPacketAsync(RespawnPacket.FromJoinGame(joinGamePacket), cancellationToken);
        else
            await link.SendPacketAsync(joinGamePacket, cancellationToken);
    }

    protected override async ValueTask<AuthenticationResult> HandleServerPacketAsync(ILink link, IMinecraftClientboundPacket packet, CancellationToken cancellationToken)
    {
        switch (packet)
        {
            case LoginPluginRequestPacket loginPluginRequestPacket:
                var result = await _events.ThrowWithResultAsync(new LoginPluginRequestEvent(link, loginPluginRequestPacket.Channel, loginPluginRequestPacket.Data), cancellationToken);
                await link.SendPacketAsync(new LoginPluginResponsePacket { Successful = result is not null, Data = result ?? [], MessageId = loginPluginRequestPacket.MessageId }, cancellationToken);
                break;
            case SetCompressionPacket:
                // handled by compression service
                break;
            case LoginDisconnectPacket loginDisconnectPacket:
                logger.LogInformation("Player {Player} cannot authenticate on {Server}: {Reason}", link.Player, link.Server, loginDisconnectPacket.Reason.SerializeLegacy());

                // since IPlayer client is already completed login state, it cannot be kicked with login disconnect packet
                await link.Player.KickAsync(loginDisconnectPacket.Reason, cancellationToken);
                return AuthenticationResult.NotAuthenticatedServer;
            case LoginSuccessPacket:
                return AuthenticationResult.Authenticated;
            default:
                throw new InvalidOperationException($"Unexpected {packet} packet received");
        }

        return AuthenticationResult.NoResult;
    }
}
