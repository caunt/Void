using Microsoft.Extensions.Logging;
using Void.Minecraft.Events;
using Void.Minecraft.Network;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Network;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Players.Contexts;
using Void.Proxy.Plugins.ExamplePlugin.Packets.Clientbound;
using Void.Proxy.Plugins.ExamplePlugin.Packets.Serverbound;

namespace Void.Proxy.Plugins.ExamplePlugin.Services;

// Here you can use DI to inject any service API you want to use.
// Since this service is Scoped, it will be instantiated per player. 
// So you can inject IPlayerContext to get access to the player instance.
public class InventoryService(IPlayerContext context, ILogger<InventoryService> logger) : IEventListener
{
    [Subscribe]
    public void OnMessageReceived(MessageReceivedEvent @event)
    {
        // Messages are considered as "Network" messages. Minecraft messages are always packets.
        // This event is fired when a packet is received from the server or from the client by proxy.

        switch (@event.Message)
        {
            case SetHeldItemServerboundPacket setHeldItemPacket:
                logger.LogInformation("Player {Player} changed held item slot to {Slot}", context.Player, setHeldItemPacket.Slot);
                break;
        }
    }

    [Subscribe]
    public void OnPhaseChanged(PhaseChangedEvent @event)
    {
        // Minecraft Phase indicate state of the game. Common Phases are Handshake, Login, Configuration and Play.
        // They are NOT synced between server and player instantly. When player is in Play phase, server might be still in Login phase.
        // This means you should decide which side Phase change you want to handle here. In this case, both sides are handled.

        switch (@event)
        {
            // Since wanted packet is in Play phase, we register it only in Play phase.
            case { Phase: Phase.Play, Side: Side.Server or Side.Client }:
                RegisterPlayMappings(@event.Player, @event.Side);
                break;
        }

        void RegisterPlayMappings(IPlayer player, Side side)
        {
            // Many packet ids and their properties can be found at
            // https://minecraft.wiki/w/Java_Edition_protocol
            // https://minecraft.wiki/w/Minecraft_Wiki:Projects/wiki.vg_merge/Protocol_version_numbers

            player.RegisterPacket<SetHeldItemServerboundPacket>([
                new(0x28, ProtocolVersion.MINECRAFT_1_20),
                new(0x2B, ProtocolVersion.MINECRAFT_1_20_2),
                new(0x2C, ProtocolVersion.MINECRAFT_1_20_3),
                new(0x2F, ProtocolVersion.MINECRAFT_1_20_5),
                new(0x31, ProtocolVersion.MINECRAFT_1_21_2),
                new(0x33, ProtocolVersion.MINECRAFT_1_21_4),
                new(0x34, ProtocolVersion.MINECRAFT_1_21_6)
            ]);

            player.RegisterPacket<SetHeldItemClientboundPacket>([
                new(0x4D, ProtocolVersion.MINECRAFT_1_20),
                new(0x4F, ProtocolVersion.MINECRAFT_1_20_2),
                new(0x51, ProtocolVersion.MINECRAFT_1_20_3),
                new(0x53, ProtocolVersion.MINECRAFT_1_20_5),
                new(0x63, ProtocolVersion.MINECRAFT_1_21_2),
                new(0x62, ProtocolVersion.MINECRAFT_1_21_5),
                new(0x67, ProtocolVersion.MINECRAFT_1_21_9)
            ]);

            context.Logger.LogTrace("Registered packet mappings at {Side} side", @event.Side);
        }
    }

    public async ValueTask ChangeSlotAsync(int slot, CancellationToken cancellationToken)
    {
        await context.Player.SendChatMessageAsync($"Your held item slot is changed to {slot}", cancellationToken);
        await context.Player.SendPacketAsync(new SetHeldItemClientboundPacket { Slot = slot }, cancellationToken);
    }
}
