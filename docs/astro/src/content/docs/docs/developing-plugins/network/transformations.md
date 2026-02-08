---
title: Transformations
description: Learn how to define packet transformations.
sidebar:
  order: 3
---

When you define a packet, it might change in future versions of Minecraft. There are two ways to handle this:
- Define conditional packet transformations
- Define flat packet transformations

:::caution
If the packet is changed in a new version of Minecraft, it is not recommended to define new classes for each packet version.

**Do not define SetHeldItemPacket_v1_20_2, SetHeldItemPacket_v1_20_3, SetHeldItemPacket_v1_20_4, etc.**

Instead, use transformations explained below.
:::


## Conditional packet transformations
Conditional packet transformations are the simplest way to handle changes, but they can become messy if the packet changes frequently.

We will use previously defined [**Set Held Item (clientbound)**](/docs/developing-plugins/network/packets/#defining-packets) packet as an example.
In this packet, changes were made from Minecraft version **1.21** to **1.21.2**.
In version **1.21**, the slot was defined as `byte`, but in version **1.21.2** it was changed to `varint`.
```csharp
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

:::note
These transformations are called **conditional** because they use simple `if` checks.

Use them only when the packet changes a few times (around 2–3 Minecraft versions). Beyond that, the code becomes hard to read.
:::

## Flat packet transformations
Flat packet transformations are more complex and harder to define, but they lead to more consistent behavior and readable code. This idea was adopted from [**ViaVersion**](https://github.com/ViaVersion/ViaVersion/) codebase.

When using these transformations, you have to keep only the **latest** implementation of the packet:
```csharp
public class SetHeldItemClientboundPacket : IMinecraftClientboundPacket<SetHeldItemClientboundPacket>
{
    public required int Slot { get; set; }

    public void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        buffer.WriteVarInt(Slot);
    }

    public static SetHeldItemClientboundPacket Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
    {
        return new SetHeldItemClientboundPacket
        {
            Slot = buffer.ReadVarInt()
        };
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
```

Now that we have the latest implementation, we need to define the changes that were made across versions.

In the case of the [**Set Held Item (clientbound)**](/docs/developing-plugins/network/packets/#defining-packets) packet, only one change was made—the `slot` property type changed from `byte` to `varint`.
```csharp
MinecraftPacketTransformationMapping[] Transformations { get; } = [
    new(ProtocolVersion.MINECRAFT_1_21, ProtocolVersion.MINECRAFT_1_21_2, wrapper =>
    {
        var slot = wrapper.Read<ByteProperty>();
        wrapper.Write(new VarIntProperty(slot.Value));
    }),
    new(ProtocolVersion.MINECRAFT_1_21_2, ProtocolVersion.MINECRAFT_1_21, wrapper =>
    {
        var slot = wrapper.Read<VarIntProperty>();
        wrapper.Write(new ByteProperty(slot.Value));
    })
];
```

Here we defined two transformations:
- From version **1.21** to **1.21.2** - we read `byte` property and write its value as `varint` property - **upgrading** our packet from previous version to next version.
- From version **1.21.2** to **1.21** - we read `varint` property and write its value as `byte` property, vice versa - **downgrading** our packet from next version to previous version.

If your packet has unchanged properties, you can use `wrapper.Passthrough()` method to skip them:
```csharp
wrapper.Passthrough<ByteProperty>();
```

Register your flat transformations for the player at the same time when you register your packet:
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
        new(0x62, ProtocolVersion.MINECRAFT_1_21_5),
        new(0x67, ProtocolVersion.MINECRAFT_1_21_9)
    ]);

    @event.Player.RegisterTransformations<SetHeldItemClientboundPacket>(Transformations);
}
```

Now the Void proxy uses your transformations to match the player's version. If the player is on version **1.21**, the proxy upgrades the packet to your latest (**1.21.2**) version and passes it to the [**Events System**](/docs/developing-plugins/events/listening-to-events/). When the plugin sends a packet to a player on an older version, the proxy downgrades it to **1.21** before sending.

:::tip
Keep defining flat transformations from one version to another when a packet changes. You only need to **define changed versions** and **skip versions without changes**. When the `slot` property is changed by Mojang to a `long` type, define another two transformations—**upgrading** and **downgrading**. Read `varint` and write `long` for an upgrade, and read `long` and write `varint` for a downgrade.
:::
