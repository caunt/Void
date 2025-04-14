using Microsoft.Extensions.Logging;
using Void.Common.Events;
using Void.Common.Network;
using Void.Minecraft.Events;
using Void.Minecraft.Links.Extensions;
using Void.Minecraft.Network;
using Void.Minecraft.Players;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Commands;
using Void.Proxy.Api.Events.Network;
using Void.Proxy.Plugins.ExamplePlugin.Packets.Clientbound;
using Void.Proxy.Plugins.ExamplePlugin.Packets.Serverbound;

namespace Void.Proxy.Plugins.ExamplePlugin.Services;

public class InventoryService(ILogger<InventoryService> logger) : IEventListener
{
    [Subscribe]
    public void OnMessageReceived(MessageReceivedEvent @event)
    {
        // This event is fired when a packet is received from the server or from the client at proxy side

        switch (@event.Message)
        {
            case SetHeldItemServerboundPacket setHeldItemPacket:
                logger.LogInformation("Player {Player} changed held item slot to {Slot}", @event.Link.Player, setHeldItemPacket.Slot);
                break;
        }
    }

    [Subscribe]
    public void OnPhaseChanged(PhaseChangedEvent @event)
    {
        switch (@event)
        {
            // Since wanted packet is in Play phase, we register it only in Play phase
            // Phase may be changed on any side and any time, so we register packets on each phase change event
            case { Phase: Phase.Play }:
                RegisterPlayMappings(@event.Player, @event.Side);
                break;
        }

        void RegisterPlayMappings(IMinecraftPlayer player, Side side)
        {
            // Many packet ids and their properties can be found at
            // https://minecraft.wiki/w/Java_Edition_protocol
            // https://minecraft.wiki/w/Minecraft_Wiki:Projects/wiki.vg_merge/Protocol_version_numbers

            player.RegisterPacket<SetHeldItemServerboundPacket>([
                new(0x2B, ProtocolVersion.MINECRAFT_1_20_2),
                new(0x2C, ProtocolVersion.MINECRAFT_1_20_3),
                new(0x2F, ProtocolVersion.MINECRAFT_1_20_5),
                new(0x31, ProtocolVersion.MINECRAFT_1_21_2),
                new(0x33, ProtocolVersion.MINECRAFT_1_21_4)
            ]);

            player.RegisterPacket<SetHeldItemClientboundPacket>([
                new(0x4F, ProtocolVersion.MINECRAFT_1_20_2),
                new(0x51, ProtocolVersion.MINECRAFT_1_20_3),
                new(0x53, ProtocolVersion.MINECRAFT_1_20_5),
                new(0x63, ProtocolVersion.MINECRAFT_1_21_2)
            ]);

            logger.LogTrace("Registered packet mappings for player {Player} at {Side} side", player, side);
        }
    }

    [Subscribe]
    public async ValueTask OnChatCommand(ChatCommandEvent @event, CancellationToken cancellationToken)
    {
        // Chat commands API is subject to change to Brigadier API

        var parts = @event.Command.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length is 0)
            return;

        switch (parts[0].ToLower())
        {
            case "slot":
                if (parts.Length is 1)
                    break;

                if (!byte.TryParse(parts[1], out var slot))
                    break;

                await @event.Link.SendPacketAsync(new SetHeldItemClientboundPacket { Slot = slot }, cancellationToken);
                @event.Result = true;

                logger.LogInformation("Changed player {Player} held item slot to {Slot}", @event.Link.Player, slot);
                break;
        }
    }
}
