---
title: Packets (Messages)
description: Learn how to define and receive Packets (Messages).
sidebar:
  order: 1
---

Minecraft Packets [**(called Messages in Void)**](https://github.com/caunt/Void/blob/main/src/Minecraft/Network/Messages/Packets/IMinecraftPacket.cs#L7) describe the data that is sent between the player and server.
These messages contain information about player actions, world events, and other game mechanics.

Typically, you describe existing packets in game, so you can read, [**modify**](/docs/developing-plugins/network/modifying-data), or cancel them.
However, you are not limited to this. You can also define your own packets for your own Minecraft mod or plugin.

Packets can be defined with `IMinecraftClientboundPacket<TPacket>` or `IMinecraftServerboundPacket<TPacket>` interface. If your packet is the same for both client and server, you can use `IMinecraftPacket<TPacket>` interface.

## Defining Packets
Your packet definition must specify how to Decode and Encode the packet data.  
In this example, we will define a [**Set Held Item (clientbound)**](https://minecraft.wiki/w/Java_Edition_protocol/Packets#Set_Held_Item_(clientbound)) packet.

This packet contains one integer property - `slot`, which is the index of the item in the player hotbar.
```csharp
// ExamplePlugin/Packets/Clientbound/SetHeldItemClientboundPacket.cs
public class SetHeldItemClientboundPacket : IMinecraftClientboundPacket<SetHeldItemClientboundPacket>
{
    public required int Slot { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        if (protocolVersion > ProtocolVersion.MINECRAFT_1_21)
            buffer.WriteVarInt(Slot);
        else
            buffer.WriteUnsignedByte((byte)Slot);
    }

    public static SetHeldItemClientboundPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        int slot;

        if (protocolVersion > ProtocolVersion.MINECRAFT_1_21)
            slot = buffer.ReadVarInt();
        else
            slot = buffer.ReadUnsignedByte();

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
Packet registrations are made for each game phase, so you need to register them in the correct phase. Common phases are `Handshake`, `Login`, `Configuration` and `Play`.

In this example we are using Void predefined packet id mappings in `PacketIdDefinitions`.
```csharp
[Subscribe]
public void OnPlayerJoinedServer(PlayerJoinedServerEvent @event)
{
    @event.Player.RegisterPacket<SetHeldItemClientboundPacket>(PacketIdDefinitions.ClientboundSetHeldItem);
}
```

You can also define your own mappings for packets you'd like to work with:
```csharp
[Subscribe]
public void OnPlayerJoinedServer(PlayerJoinedServerEvent @event)
{
    @event.Player.RegisterPacket<SetHeldItemClientboundPacket>([
        new(0x09, ProtocolVersion.Oldest),
        new(0x38, ProtocolVersion.MINECRAFT_1_8),
        new(0x37, ProtocolVersion.MINECRAFT_1_9),
        new(0x3B, ProtocolVersion.MINECRAFT_1_12),
        new(0x3A, ProtocolVersion.MINECRAFT_1_13),
        new(0x3F, ProtocolVersion.MINECRAFT_1_17),
        new(0x3C, ProtocolVersion.MINECRAFT_1_18),
        new(0x48, ProtocolVersion.MINECRAFT_1_19),
        new(0x4A, ProtocolVersion.MINECRAFT_1_19_1),
        new(0x4C, ProtocolVersion.MINECRAFT_1_19_3),
        new(0x4D, ProtocolVersion.MINECRAFT_1_20),
        new(0x4F, ProtocolVersion.MINECRAFT_1_20_2),
        new(0x51, ProtocolVersion.MINECRAFT_1_20_3),
        new(0x53, ProtocolVersion.MINECRAFT_1_20_5),
        new(0x63, ProtocolVersion.MINECRAFT_1_21_2),
        new(0x62, ProtocolVersion.MINECRAFT_1_21_5),
        new(0x67, ProtocolVersion.MINECRAFT_1_21_9)
    ]);
}
```

:::note
Each packet ID takes effect at its listed protocol version and remains valid until the next version in the table replaces it.
:::

## Receiving Packets
Now that we have defined our packet, we can receive it with the `MessageReceivedEvent` [**event**](/docs/developing-plugins/events/listening-to-events/).
```csharp
[Subscribe]
public void OnMessageReceived(MessageReceivedEvent @event)
{
    // Check if the message is a SetHeldItemClientboundPacket
    if (@event.Message is not SetHeldItemClientboundPacket packet)
        return;
        
    // Print the slot value sent by the server
    Console.WriteLine($"Received {nameof(SetHeldItemClientboundPacket)} with Slot: {packet.Slot}");
}
```

There is also a `MessageSentEvent` that is triggered when the packet is already sent to the receiver.
```csharp
[Subscribe]
public void OnMessageSent(MessageSentEvent @event)
{
    // Check if the message is a SetHeldItemClientboundPacket
    if (@event.Message is not SetHeldItemClientboundPacket packet)
        return;
        
    // Print the slot value sent to the player
    Console.WriteLine($"Sent {nameof(SetHeldItemClientboundPacket)} with Slot: {packet.Slot}");
}
```

## Sending Packets
There are 3 ways to send packets in Void:
- Directly to the `IPlayer` instance
- To the `INetworkChannel` of server or player 
- To the [**`ILink`**](/docs/developing-plugins/network/links) connection between the server and player

### Sending Packets to the Player
You can send packets to the player with `SendPacketAsync` method on `IPlayer` instance.
```csharp
await player.SendPacketAsync(new SetHeldItemClientboundPacket { Slot = slot }, cancellationToken);
```

### Sending Packets to the Server
You can send packets to the server with `ILink.ServerChannel` instance.
```csharp
await player.Link.ServerChannel.SendPacketAsync(new SetHeldItemClientboundPacket { Slot = slot }, cancellationToken);
```

### Sending Packets to the [**`ILink`**](/docs/developing-plugins/network/links)
You can send packets to the link with `ILink.SendPacketAsync` method.
[**`ILink`**](/docs/developing-plugins/network/links) will automatically determine the destination of the packet based on the packet interface.  
- If the packet has `IMinecraftClientboundPacket<TPacket>` interface, it will be sent to the client.
- If the packet has `IMinecraftServerboundPacket<TPacket>` interface, it will be sent to the server.
- If the packet has both interfaces, it will be sent only to the client.
- If the packet has neither interface, `InvalidOperationException` will be thrown.
```csharp
await player.Link.SendPacketAsync(new SetHeldItemClientboundPacket { Slot = slot }, cancellationToken);
```

When you want to explicitly send a packet to the server or client, `SendPacketAsync` has an overload that specifies the destination side.
```csharp
await player.Link.SendPacketAsync(Side.Client, new SetHeldItemClientboundPacket { Slot = slot }, cancellationToken);
```

## Complete Example
Check out [**complete example**](https://github.com/caunt/Void/blob/main/src/Plugins/ExamplePlugin/Services/InventoryService.cs) for inventory service plugin that includes both clientbound and serverbound set held item packets.


## Cancelling Packets
You can cancel packets in the `MessageReceivedEvent`.
Set the `IEvent.Result` value to `true` to prevent sending the packet to the receiver.
```csharp
[Subscribe]
public void OnMessageReceived(MessageReceivedEvent @event)
{
    if (@event.Message is not SetHeldItemClientboundPacket packet)
        return;
        
    // Cancel the SetHeldItemClientboundPacket packet
    @event.Result = true;
}
```
