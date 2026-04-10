# <a id="Void_Minecraft_Network_Registries_PacketId_Mappings_MinecraftPacketDecoder_1"></a> Delegate MinecraftPacketDecoder<TPacket\>

Namespace: [Void.Minecraft.Network.Registries.PacketId.Mappings](Void.Minecraft.Network.Registries.PacketId.Mappings.md)  
Assembly: Void.Minecraft.dll  

```csharp
public delegate TPacket MinecraftPacketDecoder<out TPacket>(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion) where TPacket : IMinecraftPacket
```

#### Parameters

`buffer` [MinecraftBuffer](Void.Minecraft.Buffers.MinecraftBuffer.md)

`protocolVersion` [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)

#### Returns

 TPacket

#### Type Parameters

`TPacket` 

