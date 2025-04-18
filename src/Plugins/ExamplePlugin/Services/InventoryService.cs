using Microsoft.Extensions.Logging;
using Void.Minecraft.Commands.Brigadier;
using Void.Minecraft.Commands.Brigadier.Builder;
using Void.Minecraft.Commands.Brigadier.Context;
using Void.Minecraft.Commands.Extensions;
using Void.Minecraft.Events;
using Void.Minecraft.Links.Extensions;
using Void.Minecraft.Network;
using Void.Minecraft.Players;
using Void.Minecraft.Players.Extensions;
using Void.Proxy.Api.Commands;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Network;
using Void.Proxy.Api.Events.Plugins;
using Void.Proxy.Api.Network;
using Void.Proxy.Plugins.ExamplePlugin.Packets.Clientbound;
using Void.Proxy.Plugins.ExamplePlugin.Packets.Serverbound;

namespace Void.Proxy.Plugins.ExamplePlugin.Services;

// Here you can use DI to inject any service API you want to use. Instance of your ExamplePlugin also can be injected.
public class InventoryService(ILogger<InventoryService> logger, ICommandService commands, ExamplePlugin plugin) : IEventListener
{
    [Subscribe]
    public void OnPluginLoad(PluginLoadEvent @event)
    {
        // This event is fired when any plugin is being loaded

        // Skip all other plugins except ours
        if (@event.Plugin != plugin)
            return;

        // Register your commands brigadier-like way
        // https://github.com/Mojang/brigadier/
        commands.Register(builder => builder
            .Literal("slot")
            .Then(builder => builder
                .Argument("slot", Arguments.Integer())
                .Executes(ChangeSlotAsync))
            .Executes(ChangeSlotAsync));
    }

    public async ValueTask<int> ChangeSlotAsync(CommandContext context, CancellationToken cancellationToken)
    {
        if (context.Source is not IMinecraftPlayer player)
        {
            logger.LogInformation("This command can be executed only by player");
            return 1;
        }

        if (!context.TryGetArgument<int>("slot", out var slot))
        {
            // If slot argument is not provided, we will use random one
            slot = Random.Shared.Next(0, 9);
        }

        await player.SendPacketAsync(new SetHeldItemClientboundPacket { Slot = slot }, cancellationToken);
        return 0;
    }

    [Subscribe]
    public void OnMessageReceived(MessageReceivedEvent @event)
    {
        // Messages are considered as "Network" messages. Minecraft messages are always packets.
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
                new(0x63, ProtocolVersion.MINECRAFT_1_21_2),
                new(0x62, ProtocolVersion.MINECRAFT_1_21_5)
            ]);

            logger.LogTrace("Registered packet mappings for player {Player} at {Side} side", player, side);
        }
    }
}
