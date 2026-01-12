using Microsoft.Extensions.Logging;
using Void.Minecraft.Events;
using Void.Minecraft.Network;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Network;
using Void.Proxy.Api.Events.Player;
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
    public async Task OnPlayerJoinedServer(PlayerJoinedServerEvent @event)
    {
        // This event triggers when player joins a server (not the proxy) and both sides become into Play phase.

        // Many packet ids and their properties can be found here:
        // https://minecraft.wiki/w/Java_Edition_protocol
        // https://minecraft.wiki/w/Minecraft_Wiki:Projects/wiki.vg_merge/Protocol_version_numbers

        // In this example we are using Void predefined packet id mappings in PacketIdDefinitions.
        // Define your own mappings for packets you'd like to work with.
        @event.Player.RegisterPacket<SetHeldItemClientboundPacket>(PacketIdDefinitions.ClientboundSetHeldItem);
        @event.Player.RegisterPacket<SetHeldItemServerboundPacket>(PacketIdDefinitions.ServerboundSetHeldItem);

        // Context logger renders log as [MinecraftPlayer <caunt>] LogMessage
        context.Logger.LogTrace("Registered packet mappings");
    }

    [Subscribe]
    public async Task OnPhaseChanged(PhaseChangedEvent @event)
    {
        // Minecraft phases indicate the state of the game. Common phases are Handshake, Login, Configuration and Play.
        // They are NOT synced between server and player instantly. When player is in Play phase, server might still be in Login phase.

        // For most packets PlayerJoinedServerEvent is sufficient to register packet mappings.
        // However if any of your packets can be delivered in unsynced phases state, you might want to register them here instead.

        context.Logger.LogDebug("Changed phase to {Phase} on {Side} side", @event.Phase, @event.Side);
    }

    public async ValueTask ChangeSlotAsync(int slot, CancellationToken cancellationToken)
    {
        await context.Player.SendChatMessageAsync($"Your held item slot is changed to {slot}", cancellationToken);
        await context.Player.SendPacketAsync(new SetHeldItemClientboundPacket { Slot = slot }, cancellationToken);
    }
}
