# <a id="Void_Minecraft_Network_Registries_PacketId_IMinecraftPacketIdSystemRegistry"></a> Interface IMinecraftPacketIdSystemRegistry

Namespace: [Void.Minecraft.Network.Registries.PacketId](Void.Minecraft.Network.Registries.PacketId.md)  
Assembly: Void.Minecraft.dll  

Defines an interface for managing and accessing Minecraft packet ID registries for different protocol versions and
operations.

```csharp
public interface IMinecraftPacketIdSystemRegistry
```

## Remarks

This interface has split <xref href="Void.Minecraft.Network.Registries.PacketId.IMinecraftPacketIdSystemRegistry.Read" data-throw-if-not-resolved="false"></xref> and <xref href="Void.Minecraft.Network.Registries.PacketId.IMinecraftPacketIdSystemRegistry.Write" data-throw-if-not-resolved="false"></xref> registries unlike <xref href="Void.Minecraft.Network.Registries.PacketId.IMinecraftPacketIdPluginsRegistry" data-throw-if-not-resolved="false"></xref>.

## Properties

### <a id="Void_Minecraft_Network_Registries_PacketId_IMinecraftPacketIdSystemRegistry_IsEmpty"></a> IsEmpty

```csharp
bool IsEmpty { get; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Network_Registries_PacketId_IMinecraftPacketIdSystemRegistry_ManagedBy"></a> ManagedBy

```csharp
IPlugin? ManagedBy { get; set; }
```

#### Property Value

 [IPlugin](Void.Proxy.Api.Plugins.IPlugin.md)?

### <a id="Void_Minecraft_Network_Registries_PacketId_IMinecraftPacketIdSystemRegistry_ProtocolVersion"></a> ProtocolVersion

```csharp
ProtocolVersion? ProtocolVersion { get; set; }
```

#### Property Value

 [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)?

### <a id="Void_Minecraft_Network_Registries_PacketId_IMinecraftPacketIdSystemRegistry_Read"></a> Read

```csharp
IMinecraftPacketIdRegistry Read { get; set; }
```

#### Property Value

 [IMinecraftPacketIdRegistry](Void.Minecraft.Network.Registries.PacketId.IMinecraftPacketIdRegistry.md)

### <a id="Void_Minecraft_Network_Registries_PacketId_IMinecraftPacketIdSystemRegistry_Write"></a> Write

```csharp
IMinecraftPacketIdRegistry Write { get; set; }
```

#### Property Value

 [IMinecraftPacketIdRegistry](Void.Minecraft.Network.Registries.PacketId.IMinecraftPacketIdRegistry.md)

## Methods

### <a id="Void_Minecraft_Network_Registries_PacketId_IMinecraftPacketIdSystemRegistry_AddPackets_Void_Proxy_Api_Network_Operation_System_Collections_Generic_IReadOnlyDictionary_Void_Minecraft_Network_Registries_PacketId_Mappings_MinecraftPacketIdMapping___System_Type__"></a> AddPackets\(Operation, IReadOnlyDictionary<MinecraftPacketIdMapping\[\], Type\>\)

```csharp
void AddPackets(Operation operation, IReadOnlyDictionary<MinecraftPacketIdMapping[], Type> mappings)
```

#### Parameters

`operation` [Operation](Void.Proxy.Api.Network.Operation.md)

`mappings` [IReadOnlyDictionary](https://learn.microsoft.com/dotnet/api/system.collections.generic.ireadonlydictionary\-2)<[MinecraftPacketIdMapping](Void.Minecraft.Network.Registries.PacketId.Mappings.MinecraftPacketIdMapping.md)\[\], [Type](https://learn.microsoft.com/dotnet/api/system.type)\>

### <a id="Void_Minecraft_Network_Registries_PacketId_IMinecraftPacketIdSystemRegistry_Contains__1"></a> Contains<T\>\(\)

```csharp
bool Contains<T>() where T : IMinecraftMessage
```

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

#### Type Parameters

`T` 

### <a id="Void_Minecraft_Network_Registries_PacketId_IMinecraftPacketIdSystemRegistry_Contains_System_Type_"></a> Contains\(Type\)

```csharp
bool Contains(Type type)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/dotnet/api/system.type)

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Network_Registries_PacketId_IMinecraftPacketIdSystemRegistry_Contains_Void_Minecraft_Network_Messages_IMinecraftMessage_"></a> Contains\(IMinecraftMessage\)

```csharp
bool Contains(IMinecraftMessage packet)
```

#### Parameters

`packet` [IMinecraftMessage](Void.Minecraft.Network.Messages.IMinecraftMessage.md)

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Network_Registries_PacketId_IMinecraftPacketIdSystemRegistry_ReplacePackets_Void_Proxy_Api_Network_Operation_System_Collections_Generic_IReadOnlyDictionary_Void_Minecraft_Network_Registries_PacketId_Mappings_MinecraftPacketIdMapping___System_Type__"></a> ReplacePackets\(Operation, IReadOnlyDictionary<MinecraftPacketIdMapping\[\], Type\>\)

```csharp
void ReplacePackets(Operation operation, IReadOnlyDictionary<MinecraftPacketIdMapping[], Type> mappings)
```

#### Parameters

`operation` [Operation](Void.Proxy.Api.Network.Operation.md)

`mappings` [IReadOnlyDictionary](https://learn.microsoft.com/dotnet/api/system.collections.generic.ireadonlydictionary\-2)<[MinecraftPacketIdMapping](Void.Minecraft.Network.Registries.PacketId.Mappings.MinecraftPacketIdMapping.md)\[\], [Type](https://learn.microsoft.com/dotnet/api/system.type)\>

### <a id="Void_Minecraft_Network_Registries_PacketId_IMinecraftPacketIdSystemRegistry_Reset"></a> Reset\(\)

```csharp
void Reset()
```

