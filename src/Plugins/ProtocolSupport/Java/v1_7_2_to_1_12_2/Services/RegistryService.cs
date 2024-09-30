using Microsoft.Extensions.DependencyInjection;
using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Network;
using Void.Proxy.API.Events.Player;
using Void.Proxy.API.Events.Plugins;
using Void.Proxy.API.Extensions;
using Void.Proxy.API.Network;
using Void.Proxy.API.Network.Protocol;
using Void.Proxy.API.Players;
using Void.Proxy.API.Plugins;
using Void.Proxy.Common.Network.Protocol;
using Void.Proxy.Common.Registries.Packets;
using Void.Proxy.Common.Services;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Packets.Serverbound;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Registries;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Services;

public class RegistryService(IPlugin plugin, IPlayerService players) : IPluginService
{
    [Subscribe]
    public void OnPlayerConnecting(PlayerConnectingEvent @event)
    {
        if (@event.Services.HasService<IPacketRegistryHolder>())
            return;

        @event.Services.AddSingleton<IPacketRegistryHolder, PacketRegistryHolder>();
    }

    [Subscribe]
    public void OnPlayerConnected(PlayerConnectedEvent @event)
    {
        if (!Plugin.SupportedVersions.Contains(@event.Player.ProtocolVersion))
            return;

        var holder = @event.Player.Context.Services.GetRequiredService<IPacketRegistryHolder>();

        if (!holder.IsEmpty)
            return;

        holder.ManagedBy = plugin;
        holder.ProtocolVersion = @event.Player.ProtocolVersion;

        SetRegistry(@event.Player, Direction.Clientbound, Mappings.ClientboundHandshakeMappings);
        SetRegistry(@event.Player, Direction.Serverbound, Mappings.ServerboundHandshakeMappings);
    }

    [Subscribe]
    public void OnMessageReceived(MessageReceivedEvent @event, CancellationToken cancellationToken)
    {
        switch (@event.Message)
        {
            case HandshakePacket handshake:
                var playerProtocolVersion = ProtocolVersion.Get(handshake.ProtocolVersion);

                if (!Plugin.SupportedVersions.Contains(playerProtocolVersion))
                    break;

                SetRegistry(@event.Link.Player, Direction.Clientbound, Mappings.ClientboundLoginMappings);
                break;
            case LoginSuccessPacket:
                SetRegistry(@event.Link.Player, Direction.Serverbound, Mappings.ServerboundPlayMappings);
                break;
        }
    }

    [Subscribe]
    public void OnMessageSent(MessageSentEvent @event, CancellationToken cancellationToken)
    {
        switch (@event.Message)
        {
            case HandshakePacket handshake:
                if (handshake.NextState == 2)
                    SetRegistry(@event.Link.Player, Direction.Serverbound, Mappings.ServerboundLoginMappings);
                else
                    ClearRegistry(@event.Link.Player);
                break;
            case LoginSuccessPacket:
                SetRegistry(@event.Link.Player, Direction.Clientbound, Mappings.ClientboundPlayMappings);
                break;
        }
    }

    [Subscribe]
    public void OnPlayerDisconnected(PlayerDisconnectedEvent @event)
    {
        ClearRegistry(@event.Player);
    }

    [Subscribe]
    public void OnPluginUnload(PluginUnloadEvent @event)
    {
        if (@event.Plugin != plugin)
            return;

        foreach (var player in players.All)
            ClearRegistry(player);
    }

    private void SetRegistry(IPlayer player, Direction direction, IReadOnlyDictionary<PacketMapping[], Type> registry)
    {
        var holder = player.Context.Services.GetRequiredService<IPacketRegistryHolder>();

        if (holder.ManagedBy != plugin)
            return;

        holder.ReplacePackets(direction, registry);
    }

    private void ClearRegistry(IPlayer player)
    {
        var holder = player.Context.Services.GetRequiredService<IPacketRegistryHolder>();

        if (holder.ManagedBy != plugin)
            return;

        holder.Reset();
    }
}