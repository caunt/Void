---
title: Modifying Data
description: Learn how to modify or cancel network messages.
sidebar:
  order: 2
---

After you have [**defined your packets**](/docs/developing-plugins/network/packets), you can modify, cancel or build and send them manually.

:::note
Void allows modifying packets in-place only for [**ILink**](/docs/developing-plugins/network/links) implementers.

However, you can still manipulate [**packets**](/docs/developing-plugins/network/packets) by canceling them and sending a modified copy manually.
In this page, we will proceed with this approach.
:::

## Modifying Packets
You can modify packets by canceling them and sending a modified copy manually.
We will use previously defined [**Set Held Item (clientbound)**](/docs/developing-plugins/network/packets#defining-packets) packet as an example.
```csharp
[Subscribe]
public async ValueTask OnMessageReceived(MessageReceivedEvent @event, CancellationToken cancellationToken)
{
    if (@event.Message is not SetHeldItemClientboundPacket packet)
        return;
    
    // Cancel the original packet
    @event.Cancel();
    
    // Modify the packet
    packet.Slot = 1;
    
    // Send the modified packet manually
    await @event.Player.SendPacketAsync(packet, cancellationToken);
}
```

:::caution
Changes to the packet properties are ignored by internal [**ILink**](/docs/developing-plugins/network/links) implementation, always ensure to call `Cancel()` and `SendPacketAsync()` to apply your changes.
:::