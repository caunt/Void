using System.Diagnostics;
using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Channels;
using Void.Proxy.API.Events.Network;
using Void.Proxy.API.Events.Services;
using Void.Proxy.API.Mojang.Minecraft.Network.Protocol;
using Void.Proxy.API.Players;
using Void.Proxy.API.Plugins;
using Void.Proxy.Plugins.Common.Extensions;
using Void.Proxy.Plugins.Common.Network.Protocol.Registries;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Packets.Serverbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_20_2_to_latest.Registries;

public class RegistryService(IPlugin plugin, IPlayerService players, IEventService events) : AbstractRegistryService(plugin, players, events)
{
    private readonly IPlugin _plugin = plugin;

    [Subscribe]
    public void OnChannelCreated(ChannelCreatedEvent @event)
    {
        if (!Plugin.SupportedVersions.Contains(@event.Initiator.ProtocolVersion))
            return;

        var registry = @event.Channel.GetPacketRegistryHolder();

        if (!registry.IsEmpty)
            return;

        registry.ManagedBy = _plugin;
        registry.ProtocolVersion = @event.Initiator.ProtocolVersion;

        var isPlayerChannel = @event.Initiator.Context.Channel is null;

        if (isPlayerChannel)
        {
            @event.Channel.SetReadingPacketsMappings(_plugin, Registry.ServerboundHandshakeMappings);
            @event.Channel.SetWritingPacketsMappings(_plugin, Registry.ClientboundHandshakeMappings);
        }
        else
        {
            @event.Channel.SetReadingPacketsMappings(_plugin, Registry.ClientboundHandshakeMappings);
            @event.Channel.SetWritingPacketsMappings(_plugin, Registry.ServerboundHandshakeMappings);
        }
    }

    [Subscribe]
    public void OnMessageReceived(MessageReceivedEvent @event)
    {
        switch (@event.Message)
        {
            case HandshakePacket handshake:
                Debug.Assert(Plugin.SupportedVersions.Contains(ProtocolVersion.Get(handshake.ProtocolVersion)));

                if (handshake.NextState is 1)
                {
                    @event.Link.PlayerChannel.SetReadingPacketsMappings(_plugin, Registry.ServerboundStatusMappings);
                    @event.Link.PlayerChannel.SetWritingPacketsMappings(_plugin, Registry.ClientboundStatusMappings);
                }
                else if (handshake.NextState is 2 or 3)
                {
                    @event.Link.PlayerChannel.SetReadingPacketsMappings(_plugin, Registry.ServerboundLoginMappings);
                    @event.Link.PlayerChannel.SetWritingPacketsMappings(_plugin, Registry.ClientboundLoginMappings);
                }

                break;
            case AcknowledgeConfigurationPacket:
            case LoginAcknowledgedPacket:
                @event.Link.PlayerChannel.SetReadingPacketsMappings(_plugin, Registry.ServerboundConfigurationMappings);
                @event.Link.PlayerChannel.SetWritingPacketsMappings(_plugin, Registry.ClientboundConfigurationMappings);
                break;
            case AcknowledgeFinishConfigurationPacket:
                @event.Link.PlayerChannel.SetReadingPacketsMappings(_plugin, Registry.ServerboundPlayMappings);
                @event.Link.PlayerChannel.SetWritingPacketsMappings(_plugin, Registry.ClientboundPlayMappings);
                break;
        }
    }

    [Subscribe]
    public void OnMessageSent(MessageSentEvent @event)
    {
        switch (@event.Message)
        {
            case HandshakePacket handshake:
                if (handshake.NextState is 1)
                {
                    @event.Link.ServerChannel.SetReadingPacketsMappings(_plugin, Registry.ClientboundStatusMappings);
                    @event.Link.ServerChannel.SetWritingPacketsMappings(_plugin, Registry.ServerboundStatusMappings);
                }
                else if (handshake.NextState is 2 or 3)
                {
                    @event.Link.ServerChannel.SetReadingPacketsMappings(_plugin, Registry.ClientboundLoginMappings);
                    @event.Link.ServerChannel.SetWritingPacketsMappings(_plugin, Registry.ServerboundLoginMappings);
                }
                else
                {
                    @event.Link.PlayerChannel.ClearPacketsMappings(_plugin);
                    @event.Link.ServerChannel.ClearPacketsMappings(_plugin);
                }

                break;
            case LoginAcknowledgedPacket:
                @event.Link.ServerChannel.SetReadingPacketsMappings(_plugin, Registry.ClientboundConfigurationMappings);
                @event.Link.ServerChannel.SetWritingPacketsMappings(_plugin, Registry.ServerboundConfigurationMappings);
                break;
            case AcknowledgeFinishConfigurationPacket:
                @event.Link.ServerChannel.SetReadingPacketsMappings(_plugin, Registry.ClientboundPlayMappings);
                @event.Link.ServerChannel.SetWritingPacketsMappings(_plugin, Registry.ServerboundPlayMappings);
                break;
        }
    }
}