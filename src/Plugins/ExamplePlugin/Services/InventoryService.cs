using Microsoft.Extensions.Logging;
using Void.Minecraft.Network;
using Void.Proxy.Api.Events;
using Void.Proxy.Api.Events.Commands;
using Void.Proxy.Api.Events.Minecraft;
using Void.Proxy.Api.Events.Network;
using Void.Proxy.Api.Links.Extensions;
using Void.Proxy.Api.Network;
using Void.Proxy.Api.Network.IO.Channels.Extensions;
using Void.Proxy.Api.Network.IO.Streams.Packet.Transformations;
using Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties;
using Void.Proxy.Api.Network.IO.Streams.Packet.Transformations.Properties.Values;
using Void.Proxy.Api.Players;
using Void.Proxy.Api.Players.Extensions;
using Void.Proxy.Plugins.ExamplePlugin.Packets.Clientbound;
using Void.Proxy.Plugins.ExamplePlugin.Packets.Serverbound;

namespace Void.Proxy.Plugins.ExamplePlugin.Services;

public class InventoryService(ILogger<InventoryService> logger) : IEventListener
{
    [Subscribe]
    public void OnMessageReceived(MessageReceivedEvent @event)
    {
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
            case { Phase: Phase.Play }:
                RegisterPlayMappings(@event.Player, @event.Side);
                break;
        }
    }

    [Subscribe]
    public async ValueTask OnChatCommand(ChatCommandEvent @event, CancellationToken cancellationToken)
    {
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

    private void RegisterPlayMappings(IPlayer player, Side side)
    {
        player.RegisterPacket<SetHeldItemServerboundPacket>([
            new(0x2B, ProtocolVersion.MINECRAFT_1_20_2),
            new(0x2C, ProtocolVersion.MINECRAFT_1_20_3),
            new(0x2F, ProtocolVersion.MINECRAFT_1_20_5),
            new(0x33, ProtocolVersion.MINECRAFT_1_21_4)
        ]);

        player.RegisterPacket<SetHeldItemClientboundPacket>([
            new(0x4F, ProtocolVersion.MINECRAFT_1_20_2),
            new(0x51, ProtocolVersion.MINECRAFT_1_20_3),
            new(0x53, ProtocolVersion.MINECRAFT_1_20_5),
            new(0x63, ProtocolVersion.MINECRAFT_1_21_4)
        ]);

        // IMinecraftPacketTransformation

        player.RegisterTransformation<SetHeldItemClientboundPacket>([
            new(ProtocolVersion.MINECRAFT_1_20_2, ProtocolVersion.MINECRAFT_1_20_3, callback),
            new(ProtocolVersion.MINECRAFT_1_20_3, ProtocolVersion.MINECRAFT_1_20_2, callback)
        ]);




        IMinecraftBinaryPacketWrapper wrapper = null!;


        var value = wrapper.Read(PropertyTypes.VarInt);
        wrapper.Write(PropertyTypes.VarInt, value);


        wrapper.Passthrough(PropertyTypes.VarInt);
        wrapper.TrySet(PropertyTypes.VarInt, 0, VarIntValue.FromPrimitive(69));





        logger.LogInformation("Registered play mappings for inventory service for {Side} side", side);
    }
}
