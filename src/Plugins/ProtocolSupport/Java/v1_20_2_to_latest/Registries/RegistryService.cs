using Microsoft.Extensions.Logging;
using Void.Minecraft.Events;
using Void.Minecraft.Network;
using Void.Minecraft.Network.Channels.Extensions;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Network;
using Void.Proxy.Api.Events.Services;
using Void.Proxy.Api.Links;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Network.Channels;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Players.Extensions;
using Void.Proxy.Api.Plugins;
using Void.Proxy.Plugins.Common.Extensions;
using Void.Proxy.Plugins.Common.Network.Packets.Clientbound;
using Void.Proxy.Plugins.Common.Network.Packets.Serverbound;
using Void.Proxy.Plugins.Common.Services.Registries;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Serverbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Registries;

public class RegistryService(ILogger<RegistryService> logger, Plugin plugin, IPlayerService players, ILinkService links, IEventService events) : AbstractRegistryService(logger, plugin, players, links, events)
{
    private readonly IPlugin _plugin = plugin;

    [Subscribe(PostOrder.First + 1)]
    public async ValueTask OnPhaseChanged(PhaseChangedEvent @event, CancellationToken cancellationToken)
    {
        if (!IsSupportedVersion(@event.Player.ProtocolVersion))
            return;

        switch (@event.Side, @event.Phase)
        {
            case (Side.Client, Phase.Handshake):
                break;
            case (Side.Client, Phase.Status):
                @event.Channel.ReplaceSystemPackets(Operation.Read, _plugin, Registry.ServerboundStatusMappings);
                @event.Channel.ReplaceSystemPackets(Operation.Write, _plugin, Registry.ClientboundStatusMappings);
                break;
            case (Side.Client, Phase.Login):
                @event.Channel.ReplaceSystemPackets(Operation.Read, _plugin, Registry.ServerboundLoginMappings);
                @event.Channel.ReplaceSystemPackets(Operation.Write, _plugin, Registry.ClientboundLoginMappings);
                break;
            case (Side.Client, Phase.Configuration):
                @event.Channel.ReplaceSystemPackets(Operation.Read, _plugin, Registry.ServerboundConfigurationMappings);
                @event.Channel.ReplaceSystemPackets(Operation.Write, _plugin, Registry.ClientboundConfigurationMappings);
                break;

            case (Side.Server, Phase.Handshake):
                break;
            case (Side.Server, Phase.Status):
                @event.Channel.ReplaceSystemPackets(Operation.Read, _plugin, Registry.ClientboundStatusMappings);
                @event.Channel.ReplaceSystemPackets(Operation.Write, _plugin, Registry.ServerboundStatusMappings);
                break;
            case (Side.Server, Phase.Login):
                @event.Channel.ReplaceSystemPackets(Operation.Read, _plugin, Registry.ClientboundLoginMappings);
                @event.Channel.ReplaceSystemPackets(Operation.Write, _plugin, Registry.ServerboundLoginMappings);
                break;
            case (Side.Server, Phase.Configuration):
                @event.Channel.ReplaceSystemPackets(Operation.Read, _plugin, Registry.ClientboundConfigurationMappings);
                @event.Channel.ReplaceSystemPackets(Operation.Write, _plugin, Registry.ServerboundConfigurationMappings);
                break;
        }
    }

    [Subscribe(PostOrder.Last)]
    public async ValueTask OnMessageReceived(MessageReceivedEvent @event, CancellationToken cancellationToken)
    {
        if (!@event.Player.IsMinecraft)
            return;

        if (@event.Message is HandshakePacket packet && !Plugin.SupportedVersions.Contains(ProtocolVersion.Get(packet.ProtocolVersion)))
            return;

        var playerChannel = await @event.Player.GetChannelAsync(cancellationToken);

        switch (@event.Message)
        {
            case HandshakePacket handshake:
                if (handshake.NextState is 1)
                    await @event.Player.SetPhaseAsync(@event.Link, Side.Client, Phase.Status, playerChannel, cancellationToken);
                else if (handshake.NextState is 2 or 3)
                    await @event.Player.SetPhaseAsync(@event.Link, Side.Client, Phase.Login, playerChannel, cancellationToken);

                break;
            case AcknowledgeConfigurationPacket:
            case LoginAcknowledgedPacket:
                await @event.Player.SetPhaseAsync(@event.Link, Side.Client, Phase.Configuration, playerChannel, cancellationToken);
                break;
        }
    }

    [Subscribe]
    public async ValueTask OnMessageSent(MessageSentEvent @event, CancellationToken cancellationToken)
    {
        if (!@event.Player.IsMinecraft)
            return;

        switch (@event.Message)
        {
            case LoginAcknowledgedPacket:
                await @event.Player.SetPhaseAsync(@event.Link, Side.Server, Phase.Configuration, @event.Link.ServerChannel, cancellationToken);
                break;
            case FinishConfigurationPacket:
                @event.Link.ServerChannel.ReplaceSystemPackets(Operation.Read, _plugin, Registry.ClientboundPlayMappings);
                @event.Link.PlayerChannel.ReplaceSystemPackets(Operation.Write, _plugin, Registry.ClientboundPlayMappings);
                await @event.Player.SetPhaseAsync(@event.Link, Side.Client, Phase.Play, @event.Link.PlayerChannel, cancellationToken);
                await @event.Player.SetPhaseAsync(@event.Link, Side.Server, Phase.Play, @event.Link.ServerChannel, cancellationToken);
                break;
            case AcknowledgeFinishConfigurationPacket:
                @event.Link.PlayerChannel.ReplaceSystemPackets(Operation.Read, _plugin, Registry.ServerboundPlayMappings);
                @event.Link.ServerChannel.ReplaceSystemPackets(Operation.Write, _plugin, Registry.ServerboundPlayMappings);
                break;
        }
    }

    protected override bool IsSupportedVersion(ProtocolVersion protocolVersion)
    {
        return Plugin.SupportedVersions.Contains(protocolVersion);
    }

    protected override void SetupRegistries(INetworkChannel channel, Side side, ProtocolVersion protocolVersion)
    {
        channel.MinecraftRegistries.Setup(_plugin, protocolVersion);

        if (side is Side.Client)
        {
            channel.ReplaceSystemPackets(Operation.Read, _plugin, Registry.ServerboundHandshakeMappings);
            channel.ReplaceSystemPackets(Operation.Write, _plugin, Registry.ClientboundHandshakeMappings);
        }
        else
        {
            channel.ReplaceSystemPackets(Operation.Read, _plugin, Registry.ClientboundHandshakeMappings);
            channel.ReplaceSystemPackets(Operation.Write, _plugin, Registry.ServerboundHandshakeMappings);
        }
    }
}
