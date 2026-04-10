# <a id="Void_Minecraft_Network_Registries_PacketId_IMinecraftPacketIdRegistry"></a> Interface IMinecraftPacketIdRegistry

Namespace: [Void.Minecraft.Network.Registries.PacketId](Void.Minecraft.Network.Registries.PacketId.md)  
Assembly: Void.Minecraft.dll  

```csharp
public interface IMinecraftPacketIdRegistry
```

#### Extension Methods

[MinecraftPacketIdRegistryExtensions.RegisterPacket<T\>\(IMinecraftPacketIdRegistry, ProtocolVersion, params MinecraftPacketIdMapping\[\]\)](Void.Minecraft.Network.Registries.PacketId.Extensions.MinecraftPacketIdRegistryExtensions.md\#Void\_Minecraft\_Network\_Registries\_PacketId\_Extensions\_MinecraftPacketIdRegistryExtensions\_RegisterPacket\_\_1\_Void\_Minecraft\_Network\_Registries\_PacketId\_IMinecraftPacketIdRegistry\_Void\_Minecraft\_Network\_ProtocolVersion\_Void\_Minecraft\_Network\_Registries\_PacketId\_Mappings\_MinecraftPacketIdMapping\_\_\_)

## Properties

### <a id="Void_Minecraft_Network_Registries_PacketId_IMinecraftPacketIdRegistry_IsEmpty"></a> IsEmpty

```csharp
bool IsEmpty { get; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Network_Registries_PacketId_IMinecraftPacketIdRegistry_PacketTypes"></a> PacketTypes

```csharp
IEnumerable<Type> PacketTypes { get; }
```

#### Property Value

 [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[Type](https://learn.microsoft.com/dotnet/api/system.type)\>

## Methods

### <a id="Void_Minecraft_Network_Registries_PacketId_IMinecraftPacketIdRegistry_AddPackets_System_Collections_Generic_IReadOnlyDictionary_Void_Minecraft_Network_Registries_PacketId_Mappings_MinecraftPacketIdMapping___System_Type__Void_Minecraft_Network_ProtocolVersion_"></a> AddPackets\(IReadOnlyDictionary<MinecraftPacketIdMapping\[\], Type\>, ProtocolVersion\)

```csharp
IMinecraftPacketIdRegistry AddPackets(IReadOnlyDictionary<MinecraftPacketIdMapping[], Type> mappings, ProtocolVersion protocolVersion)
```

#### Parameters

`mappings` [IReadOnlyDictionary](https://learn.microsoft.com/dotnet/api/system.collections.generic.ireadonlydictionary\-2)<[MinecraftPacketIdMapping](Void.Minecraft.Network.Registries.PacketId.Mappings.MinecraftPacketIdMapping.md)\[\], [Type](https://learn.microsoft.com/dotnet/api/system.type)\>

`protocolVersion` [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)

#### Returns

 [IMinecraftPacketIdRegistry](Void.Minecraft.Network.Registries.PacketId.IMinecraftPacketIdRegistry.md)

### <a id="Void_Minecraft_Network_Registries_PacketId_IMinecraftPacketIdRegistry_Clear"></a> Clear\(\)

```csharp
void Clear()
```

### <a id="Void_Minecraft_Network_Registries_PacketId_IMinecraftPacketIdRegistry_Clear_Void_Proxy_Api_Network_Direction_"></a> Clear\(Direction\)

```csharp
void Clear(Direction direction)
```

#### Parameters

`direction` [Direction](Void.Proxy.Api.Network.Direction.md)

### <a id="Void_Minecraft_Network_Registries_PacketId_IMinecraftPacketIdRegistry_Contains__1"></a> Contains<T\>\(\)

```csharp
bool Contains<T>() where T : IMinecraftPacket
```

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

#### Type Parameters

`T` 

### <a id="Void_Minecraft_Network_Registries_PacketId_IMinecraftPacketIdRegistry_Contains_Void_Proxy_Api_Network_Messages_INetworkMessage_"></a> Contains\(INetworkMessage\)

```csharp
bool Contains(INetworkMessage message)
```

#### Parameters

`message` [INetworkMessage](Void.Proxy.Api.Network.Messages.INetworkMessage.md)

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Network_Registries_PacketId_IMinecraftPacketIdRegistry_Contains_System_Type_"></a> Contains\(Type\)

```csharp
bool Contains(Type type)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/dotnet/api/system.type)

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Network_Registries_PacketId_IMinecraftPacketIdRegistry_ReplacePackets_System_Collections_Generic_IReadOnlyDictionary_Void_Minecraft_Network_Registries_PacketId_Mappings_MinecraftPacketIdMapping___System_Type__Void_Minecraft_Network_ProtocolVersion_"></a> ReplacePackets\(IReadOnlyDictionary<MinecraftPacketIdMapping\[\], Type\>, ProtocolVersion\)

```csharp
IMinecraftPacketIdRegistry ReplacePackets(IReadOnlyDictionary<MinecraftPacketIdMapping[], Type> mappings, ProtocolVersion protocolVersion)
```

#### Parameters

`mappings` [IReadOnlyDictionary](https://learn.microsoft.com/dotnet/api/system.collections.generic.ireadonlydictionary\-2)<[MinecraftPacketIdMapping](Void.Minecraft.Network.Registries.PacketId.Mappings.MinecraftPacketIdMapping.md)\[\], [Type](https://learn.microsoft.com/dotnet/api/system.type)\>

`protocolVersion` [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)

#### Returns

 [IMinecraftPacketIdRegistry](Void.Minecraft.Network.Registries.PacketId.IMinecraftPacketIdRegistry.md)

### <a id="Void_Minecraft_Network_Registries_PacketId_IMinecraftPacketIdRegistry_TryCreateDecoder_System_Int32_Void_Minecraft_Network_Registries_PacketId_Mappings_MinecraftPacketDecoder_Void_Minecraft_Network_Messages_Packets_IMinecraftPacket___"></a> TryCreateDecoder\(int, out MinecraftPacketDecoder<IMinecraftPacket\>\)

```csharp
bool TryCreateDecoder(int id, out MinecraftPacketDecoder<IMinecraftPacket> packet)
```

#### Parameters

`id` [int](https://learn.microsoft.com/dotnet/api/system.int32)

`packet` [MinecraftPacketDecoder](Void.Minecraft.Network.Registries.PacketId.Mappings.MinecraftPacketDecoder\-1.md)<[IMinecraftPacket](Void.Minecraft.Network.Messages.Packets.IMinecraftPacket.md)\>

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Network_Registries_PacketId_IMinecraftPacketIdRegistry_TryCreateDecoder_System_Int32_System_Type__Void_Minecraft_Network_Registries_PacketId_Mappings_MinecraftPacketDecoder_Void_Minecraft_Network_Messages_Packets_IMinecraftPacket___"></a> TryCreateDecoder\(int, out Type, out MinecraftPacketDecoder<IMinecraftPacket\>\)

```csharp
bool TryCreateDecoder(int id, out Type packetType, out MinecraftPacketDecoder<IMinecraftPacket> packet)
```

#### Parameters

`id` [int](https://learn.microsoft.com/dotnet/api/system.int32)

`packetType` [Type](https://learn.microsoft.com/dotnet/api/system.type)

`packet` [MinecraftPacketDecoder](Void.Minecraft.Network.Registries.PacketId.Mappings.MinecraftPacketDecoder\-1.md)<[IMinecraftPacket](Void.Minecraft.Network.Messages.Packets.IMinecraftPacket.md)\>

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Network_Registries_PacketId_IMinecraftPacketIdRegistry_TryGetPacketId_Void_Minecraft_Network_Messages_Packets_IMinecraftPacket_System_Int32__"></a> TryGetPacketId\(IMinecraftPacket, out int\)

```csharp
bool TryGetPacketId(IMinecraftPacket packet, out int id)
```

#### Parameters

`packet` [IMinecraftPacket](Void.Minecraft.Network.Messages.Packets.IMinecraftPacket.md)

`id` [int](https://learn.microsoft.com/dotnet/api/system.int32)

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Network_Registries_PacketId_IMinecraftPacketIdRegistry_TryGetType_System_Int32_System_Type__"></a> TryGetType\(int, out Type\)

```csharp
bool TryGetType(int id, out Type packetType)
```

#### Parameters

`id` [int](https://learn.microsoft.com/dotnet/api/system.int32)

`packetType` [Type](https://learn.microsoft.com/dotnet/api/system.type)

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

