# <a id="Void_Minecraft_Network_Registries_Transformations_IMinecraftPacketTransformationsSystemRegistry"></a> Interface IMinecraftPacketTransformationsSystemRegistry

Namespace: [Void.Minecraft.Network.Registries.Transformations](Void.Minecraft.Network.Registries.Transformations.md)  
Assembly: Void.Minecraft.dll  

```csharp
public interface IMinecraftPacketTransformationsSystemRegistry
```

## Properties

### <a id="Void_Minecraft_Network_Registries_Transformations_IMinecraftPacketTransformationsSystemRegistry_All"></a> All

```csharp
IMinecraftPacketTransformationsRegistry All { get; }
```

#### Property Value

 [IMinecraftPacketTransformationsRegistry](Void.Minecraft.Network.Registries.Transformations.IMinecraftPacketTransformationsRegistry.md)

### <a id="Void_Minecraft_Network_Registries_Transformations_IMinecraftPacketTransformationsSystemRegistry_IsEmpty"></a> IsEmpty

```csharp
bool IsEmpty { get; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Network_Registries_Transformations_IMinecraftPacketTransformationsSystemRegistry_ManagedBy"></a> ManagedBy

```csharp
IPlugin? ManagedBy { get; set; }
```

#### Property Value

 [IPlugin](Void.Proxy.Api.Plugins.IPlugin.md)?

### <a id="Void_Minecraft_Network_Registries_Transformations_IMinecraftPacketTransformationsSystemRegistry_ProtocolVersion"></a> ProtocolVersion

```csharp
ProtocolVersion? ProtocolVersion { get; set; }
```

#### Property Value

 [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)?

## Methods

### <a id="Void_Minecraft_Network_Registries_Transformations_IMinecraftPacketTransformationsSystemRegistry_Clear"></a> Clear\(\)

```csharp
void Clear()
```

### <a id="Void_Minecraft_Network_Registries_Transformations_IMinecraftPacketTransformationsSystemRegistry_Contains__1_Void_Minecraft_Network_Registries_Transformations_Mappings_TransformationType_"></a> Contains<T\>\(TransformationType\)

```csharp
bool Contains<T>(TransformationType type) where T : IMinecraftPacket
```

#### Parameters

`type` [TransformationType](Void.Minecraft.Network.Registries.Transformations.Mappings.TransformationType.md)

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

#### Type Parameters

`T` 

### <a id="Void_Minecraft_Network_Registries_Transformations_IMinecraftPacketTransformationsSystemRegistry_Contains_Void_Minecraft_Network_Messages_IMinecraftMessage_Void_Minecraft_Network_Registries_Transformations_Mappings_TransformationType_"></a> Contains\(IMinecraftMessage, TransformationType\)

```csharp
bool Contains(IMinecraftMessage message, TransformationType type)
```

#### Parameters

`message` [IMinecraftMessage](Void.Minecraft.Network.Messages.IMinecraftMessage.md)

`type` [TransformationType](Void.Minecraft.Network.Registries.Transformations.Mappings.TransformationType.md)

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Network_Registries_Transformations_IMinecraftPacketTransformationsSystemRegistry_Contains_System_Type_Void_Minecraft_Network_Registries_Transformations_Mappings_TransformationType_"></a> Contains\(Type, TransformationType\)

```csharp
bool Contains(Type packetType, TransformationType transformationType)
```

#### Parameters

`packetType` [Type](https://learn.microsoft.com/dotnet/api/system.type)

`transformationType` [TransformationType](Void.Minecraft.Network.Registries.Transformations.Mappings.TransformationType.md)

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Network_Registries_Transformations_IMinecraftPacketTransformationsSystemRegistry_Reset"></a> Reset\(\)

```csharp
void Reset()
```

