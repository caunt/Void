# <a id="Void_Minecraft_Network_Messages_Packets_IMinecraftPacket_1"></a> Interface IMinecraftPacket<TSelf\>

Namespace: [Void.Minecraft.Network.Messages.Packets](Void.Minecraft.Network.Messages.Packets.md)  
Assembly: Void.Minecraft.dll  

```csharp
public interface IMinecraftPacket<out TSelf> : IMinecraftPacket, IMinecraftMessage, INetworkMessage, IDisposable where TSelf : IMinecraftPacket
```

#### Type Parameters

`TSelf` 

#### Implements

[IMinecraftPacket](Void.Minecraft.Network.Messages.Packets.IMinecraftPacket.md), 
[IMinecraftMessage](Void.Minecraft.Network.Messages.IMinecraftMessage.md), 
[INetworkMessage](Void.Proxy.Api.Network.Messages.INetworkMessage.md), 
[IDisposable](https://learn.microsoft.com/dotnet/api/system.idisposable)

## Methods

### <a id="Void_Minecraft_Network_Messages_Packets_IMinecraftPacket_1_Decode_Void_Minecraft_Buffers_MinecraftBuffer__Void_Minecraft_Network_ProtocolVersion_"></a> Decode\(ref MinecraftBuffer, ProtocolVersion\)

```csharp
public static abstract TSelf Decode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
```

#### Parameters

`buffer` [MinecraftBuffer](Void.Minecraft.Buffers.MinecraftBuffer.md)

`protocolVersion` [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)

#### Returns

 TSelf

