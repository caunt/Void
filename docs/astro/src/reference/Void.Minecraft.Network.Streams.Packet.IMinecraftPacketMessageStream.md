# <a id="Void_Minecraft_Network_Streams_Packet_IMinecraftPacketMessageStream"></a> Interface IMinecraftPacketMessageStream

Namespace: [Void.Minecraft.Network.Streams.Packet](Void.Minecraft.Network.Streams.Packet.md)  
Assembly: Void.Minecraft.dll  

```csharp
public interface IMinecraftPacketMessageStream : IMinecraftStream, IMessageStream, IMessageStreamBase, IDisposable, IAsyncDisposable
```

#### Implements

[IMinecraftStream](Void.Minecraft.Network.Streams.IMinecraftStream.md), 
[IMessageStream](Void.Proxy.Api.Network.Streams.IMessageStream.md), 
[IMessageStreamBase](Void.Proxy.Api.Network.Streams.IMessageStreamBase.md), 
[IDisposable](https://learn.microsoft.com/dotnet/api/system.idisposable), 
[IAsyncDisposable](https://learn.microsoft.com/dotnet/api/system.iasyncdisposable)

## Properties

### <a id="Void_Minecraft_Network_Streams_Packet_IMinecraftPacketMessageStream_Registries"></a> Registries

```csharp
IRegistryHolder Registries { get; }
```

#### Property Value

 [IRegistryHolder](Void.Minecraft.Network.Registries.IRegistryHolder.md)

## Methods

### <a id="Void_Minecraft_Network_Streams_Packet_IMinecraftPacketMessageStream_ReadPacket"></a> ReadPacket\(\)

```csharp
IMinecraftPacket ReadPacket()
```

#### Returns

 [IMinecraftPacket](Void.Minecraft.Network.Messages.Packets.IMinecraftPacket.md)

### <a id="Void_Minecraft_Network_Streams_Packet_IMinecraftPacketMessageStream_ReadPacketAsync_System_Threading_CancellationToken_"></a> ReadPacketAsync\(CancellationToken\)

```csharp
ValueTask<IMinecraftPacket> ReadPacketAsync(CancellationToken cancellationToken = default)
```

#### Parameters

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask\-1)<[IMinecraftPacket](Void.Minecraft.Network.Messages.Packets.IMinecraftPacket.md)\>

### <a id="Void_Minecraft_Network_Streams_Packet_IMinecraftPacketMessageStream_WritePacket_Void_Minecraft_Network_Messages_Packets_IMinecraftPacket_"></a> WritePacket\(IMinecraftPacket\)

```csharp
void WritePacket(IMinecraftPacket packet)
```

#### Parameters

`packet` [IMinecraftPacket](Void.Minecraft.Network.Messages.Packets.IMinecraftPacket.md)

### <a id="Void_Minecraft_Network_Streams_Packet_IMinecraftPacketMessageStream_WritePacketAsync_Void_Minecraft_Network_Messages_Packets_IMinecraftPacket_System_Threading_CancellationToken_"></a> WritePacketAsync\(IMinecraftPacket, CancellationToken\)

```csharp
ValueTask WritePacketAsync(IMinecraftPacket packet, CancellationToken cancellationToken = default)
```

#### Parameters

`packet` [IMinecraftPacket](Void.Minecraft.Network.Messages.Packets.IMinecraftPacket.md)

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask)

