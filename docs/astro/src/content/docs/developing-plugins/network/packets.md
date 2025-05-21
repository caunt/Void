---
title: Packets (Messages)
description: Learn how to define and receive Packets (Messages).
sidebar:
  order: 1
---

Minecraft Packets [**(called Messages in Void)**](https://github.com/caunt/Void/blob/main/src/Minecraft/Network/Messages/Packets/IMinecraftPacket.cs#L7) describe the data that is sent between the player and server.
These messages contain information about player actions, world events, and other game mechanics.

Typically, you describe existing packets in game, so you can read, modify, or cancel them.  
However you are not limited to this, you can also define your own packets for your own minecraft mod or plugin.

Packets can be defined with `IMinecraftClientboundPacket<TPacket>` or `IMinecraftServerboundPacket<TPacket>` interface. If your packet is same for both client and server, you can use `IMinecraftPacket<TPacket>` interface.

## Defining Packets
Your packet definition must specify how to Decode and Encode the packet data.  
In this example, we will define a [Set Held Item (clientbound)](https://minecraft.wiki/w/Java_Edition_protocol/Packets#Set_Held_Item_(clientbound)) packet.

This packet contains one integer property - `slot`, which is the index of the item in the player hotbar.
```csharp
// ExamplePlugin/Packets/Clientbound/SetHeldItemClientboundPacket.cs
public class SetHeldItemClientboundPacket : IMinecraftClientboundPacket<SetHeldItemClientboundPacket>
{
    public required int Slot { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteVarInt(Slot);
    }

    public static SetHeldItemClientboundPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        int slot = buffer.ReadVarInt();

        return new SetHeldItemClientboundPacket
        {
            Slot = slot
        };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}

```

## Registering Packets
Before receiving or sending packets, you need to register them specifying packet ids for each game protocol version.
Packets registrations are made for each game phase, so you need to register them in the correct phase. Common phases are `Handshake`, `Login`, `Configuration` and `Play`.
```csharp
[Subscribe]
public void OnPhaseChanged(PhaseChangedEvent @event)
{
    if (@event.Phase is not Phase.Play)
        return;

    @event.Player.RegisterPacket<SetHeldItemClientboundPacket>([
        new(0x4F, ProtocolVersion.MINECRAFT_1_20_2),
        new(0x51, ProtocolVersion.MINECRAFT_1_20_3),
        new(0x53, ProtocolVersion.MINECRAFT_1_20_5),
        new(0x63, ProtocolVersion.MINECRAFT_1_21_2),
        new(0x62, ProtocolVersion.MINECRAFT_1_21_5)
    ]);
}
```

:::note
ProtocolVersion's are specified in 'starting from' context.
So the 0x4F packet id for `ProtocolVersion.MINECRAFT_1_20_2` means that this packet id is used starting from that version included and up to next version in this mapping definition excluded - `ProtocolVersion.MINECRAFT_1_20_3`, so basically just one 1.20.2 version.
:::

## Receiving Packets
Now that we have defined our packet, we can receive it with `MessageReceivedEvent` [**event**](../../events/listening-to-events/).
```csharp
[Subscribe]
public void OnMessageReceived(MessageReceivedEvent @event)
{
    // Check if the message is a SetHeldItemClientboundPacket
    if (@event.Message is not SetHeldItemClientboundPacket packet)
        return;
        
    // Print the slot value sent by server
    Console.WriteLine($"Received {nameof(SetHeldItemClientboundPacket)} with Slot: {packet.Slot}");
}
```

## Sending Packets
There are 3 ways to send packets in Void:
- Directly to the `IPlayer` instance
- To the server / player `IMinecraftChannel`
- To the `ILink` connected between the server and player

### Sending Packets to the Player
You can send packets manually to the player with `SendPacketAsync` method on `IPlayer` instance.
```csharp
await player.SendPacketAsync(new SetHeldItemClientboundPacket { Slot = slot }, cancellationToken);
```

### Sending Packets to the Server
You can send packets manually to the server with `ILink.ServerChannel` instance.
```csharp
await player.GetLink().ServerChannel.SendPacketAsync(new SetHeldItemClientboundPacket { Slot = slot }, cancellationToken);
```

### Sending Packets to the Link
You can send packets manually to the link with `ILink.SendPacketAsync` method.
`ILink` will automatically determine the destination of the packet based on the packet interface.  
- If the packet has `IMinecraftClientboundPacket<TPacket>` interface, it will be sent to the client.
- If the packet has `IMinecraftServerboundPacket<TPacket>` interface, it will be sent to the server.
- If the packet has both interfaces, it will be sent only to the client.
- If the packet has neither interface, `InvalidOperationException` will be thrown.
```csharp
await player.GetLink().SendPacketAsync(new SetHeldItemClientboundPacket { Slot = slot }, cancellationToken);
```

When you want to explicitly send a packet to the server or client, `SendPacketAsync` has overloads that specifies the Side of destination.
```csharp
await player.GetLink().SendPacketAsync(Side.Client, new SetHeldItemClientboundPacket { Slot = slot }, cancellationToken);
```

## Complete Example
Check out [**complete example**](https://github.com/caunt/Void/blob/main/src/Plugins/ExamplePlugin/Services/InventoryService.cs) for inventory service plugin that includes both clientbound and serverbound set held item packets.
