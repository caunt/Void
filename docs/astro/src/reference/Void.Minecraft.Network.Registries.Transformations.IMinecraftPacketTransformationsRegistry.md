# <a id="Void_Minecraft_Network_Registries_Transformations_IMinecraftPacketTransformationsRegistry"></a> Interface IMinecraftPacketTransformationsRegistry

Namespace: [Void.Minecraft.Network.Registries.Transformations](Void.Minecraft.Network.Registries.Transformations.md)  
Assembly: Void.Minecraft.dll  

```csharp
public interface IMinecraftPacketTransformationsRegistry
```

#### Extension Methods

[MinecraftPacketTransformationsExtensions.RegisterTransformations<T\>\(IMinecraftPacketTransformationsRegistry, ProtocolVersion, params IEnumerable<MinecraftPacketTransformationMapping\>\)](Void.Minecraft.Network.Registries.Transformations.Extensions.MinecraftPacketTransformationsExtensions.md\#Void\_Minecraft\_Network\_Registries\_Transformations\_Extensions\_MinecraftPacketTransformationsExtensions\_RegisterTransformations\_\_1\_Void\_Minecraft\_Network\_Registries\_Transformations\_IMinecraftPacketTransformationsRegistry\_Void\_Minecraft\_Network\_ProtocolVersion\_System\_Collections\_Generic\_IEnumerable\_Void\_Minecraft\_Network\_Registries\_Transformations\_Mappings\_MinecraftPacketTransformationMapping\_\_)

## Properties

### <a id="Void_Minecraft_Network_Registries_Transformations_IMinecraftPacketTransformationsRegistry_IsEmpty"></a> IsEmpty

```csharp
bool IsEmpty { get; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Network_Registries_Transformations_IMinecraftPacketTransformationsRegistry_PacketTypes"></a> PacketTypes

```csharp
IEnumerable<Type> PacketTypes { get; }
```

#### Property Value

 [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[Type](https://learn.microsoft.com/dotnet/api/system.type)\>

## Methods

### <a id="Void_Minecraft_Network_Registries_Transformations_IMinecraftPacketTransformationsRegistry_Add_System_Collections_Generic_IReadOnlyDictionary_System_Collections_Generic_IEnumerable_Void_Minecraft_Network_Registries_Transformations_Mappings_MinecraftPacketTransformationMapping__System_Type__Void_Minecraft_Network_ProtocolVersion_"></a> Add\(IReadOnlyDictionary<IEnumerable<MinecraftPacketTransformationMapping\>, Type\>, ProtocolVersion\)

```csharp
IMinecraftPacketTransformationsRegistry Add(IReadOnlyDictionary<IEnumerable<MinecraftPacketTransformationMapping>, Type> mappings, ProtocolVersion protocolVersion)
```

#### Parameters

`mappings` [IReadOnlyDictionary](https://learn.microsoft.com/dotnet/api/system.collections.generic.ireadonlydictionary\-2)<[IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[MinecraftPacketTransformationMapping](Void.Minecraft.Network.Registries.Transformations.Mappings.MinecraftPacketTransformationMapping.md)\>, [Type](https://learn.microsoft.com/dotnet/api/system.type)\>

`protocolVersion` [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)

#### Returns

 [IMinecraftPacketTransformationsRegistry](Void.Minecraft.Network.Registries.Transformations.IMinecraftPacketTransformationsRegistry.md)

### <a id="Void_Minecraft_Network_Registries_Transformations_IMinecraftPacketTransformationsRegistry_Clear"></a> Clear\(\)

```csharp
void Clear()
```

### <a id="Void_Minecraft_Network_Registries_Transformations_IMinecraftPacketTransformationsRegistry_Clear_Void_Proxy_Api_Network_Direction_"></a> Clear\(Direction\)

```csharp
void Clear(Direction direction)
```

#### Parameters

`direction` [Direction](Void.Proxy.Api.Network.Direction.md)

### <a id="Void_Minecraft_Network_Registries_Transformations_IMinecraftPacketTransformationsRegistry_Contains__1_Void_Minecraft_Network_Registries_Transformations_Mappings_TransformationType_"></a> Contains<T\>\(TransformationType\)

```csharp
bool Contains<T>(TransformationType type) where T : IMinecraftPacket
```

#### Parameters

`type` [TransformationType](Void.Minecraft.Network.Registries.Transformations.Mappings.TransformationType.md)

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

#### Type Parameters

`T` 

### <a id="Void_Minecraft_Network_Registries_Transformations_IMinecraftPacketTransformationsRegistry_Contains_Void_Minecraft_Network_Messages_IMinecraftMessage_Void_Minecraft_Network_Registries_Transformations_Mappings_TransformationType_"></a> Contains\(IMinecraftMessage, TransformationType\)

```csharp
bool Contains(IMinecraftMessage message, TransformationType type)
```

#### Parameters

`message` [IMinecraftMessage](Void.Minecraft.Network.Messages.IMinecraftMessage.md)

`type` [TransformationType](Void.Minecraft.Network.Registries.Transformations.Mappings.TransformationType.md)

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Network_Registries_Transformations_IMinecraftPacketTransformationsRegistry_Contains_System_Type_Void_Minecraft_Network_Registries_Transformations_Mappings_TransformationType_"></a> Contains\(Type, TransformationType\)

```csharp
bool Contains(Type packetType, TransformationType transformationType)
```

#### Parameters

`packetType` [Type](https://learn.microsoft.com/dotnet/api/system.type)

`transformationType` [TransformationType](Void.Minecraft.Network.Registries.Transformations.Mappings.TransformationType.md)

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Network_Registries_Transformations_IMinecraftPacketTransformationsRegistry_Replace_System_Collections_Generic_IReadOnlyDictionary_System_Collections_Generic_IEnumerable_Void_Minecraft_Network_Registries_Transformations_Mappings_MinecraftPacketTransformationMapping__System_Type__Void_Minecraft_Network_ProtocolVersion_"></a> Replace\(IReadOnlyDictionary<IEnumerable<MinecraftPacketTransformationMapping\>, Type\>, ProtocolVersion\)

```csharp
IMinecraftPacketTransformationsRegistry Replace(IReadOnlyDictionary<IEnumerable<MinecraftPacketTransformationMapping>, Type> mappings, ProtocolVersion protocolVersion)
```

#### Parameters

`mappings` [IReadOnlyDictionary](https://learn.microsoft.com/dotnet/api/system.collections.generic.ireadonlydictionary\-2)<[IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[MinecraftPacketTransformationMapping](Void.Minecraft.Network.Registries.Transformations.Mappings.MinecraftPacketTransformationMapping.md)\>, [Type](https://learn.microsoft.com/dotnet/api/system.type)\>

`protocolVersion` [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)

#### Returns

 [IMinecraftPacketTransformationsRegistry](Void.Minecraft.Network.Registries.Transformations.IMinecraftPacketTransformationsRegistry.md)

### <a id="Void_Minecraft_Network_Registries_Transformations_IMinecraftPacketTransformationsRegistry_TryGetFor_System_Type_Void_Minecraft_Network_Registries_Transformations_Mappings_TransformationType_Void_Minecraft_Network_Registries_Transformations_Mappings_MinecraftPacketTransformation____"></a> TryGetFor\(Type, TransformationType, out MinecraftPacketTransformation\[\]\)

```csharp
bool TryGetFor(Type packetType, TransformationType type, out MinecraftPacketTransformation[] transformation)
```

#### Parameters

`packetType` [Type](https://learn.microsoft.com/dotnet/api/system.type)

`type` [TransformationType](Void.Minecraft.Network.Registries.Transformations.Mappings.TransformationType.md)

`transformation` [MinecraftPacketTransformation](Void.Minecraft.Network.Registries.Transformations.Mappings.MinecraftPacketTransformation.md)\[\]

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

