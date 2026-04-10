# <a id="Void_Minecraft_Network_Messages_Packets_IMinecraftPacket"></a> Interface IMinecraftPacket

Namespace: [Void.Minecraft.Network.Messages.Packets](Void.Minecraft.Network.Messages.Packets.md)  
Assembly: Void.Minecraft.dll  

```csharp
public interface IMinecraftPacket : IMinecraftMessage, INetworkMessage, IDisposable
```

#### Implements

[IMinecraftMessage](Void.Minecraft.Network.Messages.IMinecraftMessage.md), 
[INetworkMessage](Void.Proxy.Api.Network.Messages.INetworkMessage.md), 
[IDisposable](https://learn.microsoft.com/dotnet/api/system.idisposable)

## Methods

### <a id="Void_Minecraft_Network_Messages_Packets_IMinecraftPacket_Encode_Void_Minecraft_Buffers_MinecraftBuffer__Void_Minecraft_Network_ProtocolVersion_"></a> Encode\(ref MinecraftBuffer, ProtocolVersion\)

```csharp
void Encode(ref MinecraftBuffer buffer, ProtocolVersion protocolVersion)
```

#### Parameters

`buffer` [MinecraftBuffer](Void.Minecraft.Buffers.MinecraftBuffer.md)

`protocolVersion` [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)

