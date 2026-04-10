# <a id="Void_Minecraft_Network_Registries_Transformations_IMinecraftPacketTransformationsPluginsRegistry"></a> Interface IMinecraftPacketTransformationsPluginsRegistry

Namespace: [Void.Minecraft.Network.Registries.Transformations](Void.Minecraft.Network.Registries.Transformations.md)  
Assembly: Void.Minecraft.dll  

```csharp
public interface IMinecraftPacketTransformationsPluginsRegistry
```

## Properties

### <a id="Void_Minecraft_Network_Registries_Transformations_IMinecraftPacketTransformationsPluginsRegistry_All"></a> All

```csharp
IReadOnlyCollection<IMinecraftPacketTransformationsRegistry> All { get; }
```

#### Property Value

 [IReadOnlyCollection](https://learn.microsoft.com/dotnet/api/system.collections.generic.ireadonlycollection\-1)<[IMinecraftPacketTransformationsRegistry](Void.Minecraft.Network.Registries.Transformations.IMinecraftPacketTransformationsRegistry.md)\>

### <a id="Void_Minecraft_Network_Registries_Transformations_IMinecraftPacketTransformationsPluginsRegistry_IsEmpty"></a> IsEmpty

```csharp
bool IsEmpty { get; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Network_Registries_Transformations_IMinecraftPacketTransformationsPluginsRegistry_ManagedBy"></a> ManagedBy

```csharp
IPlugin? ManagedBy { get; set; }
```

#### Property Value

 [IPlugin](Void.Proxy.Api.Plugins.IPlugin.md)?

### <a id="Void_Minecraft_Network_Registries_Transformations_IMinecraftPacketTransformationsPluginsRegistry_ProtocolVersion"></a> ProtocolVersion

```csharp
ProtocolVersion? ProtocolVersion { get; set; }
```

#### Property Value

 [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)?

## Methods

### <a id="Void_Minecraft_Network_Registries_Transformations_IMinecraftPacketTransformationsPluginsRegistry_Clear"></a> Clear\(\)

```csharp
void Clear()
```

### <a id="Void_Minecraft_Network_Registries_Transformations_IMinecraftPacketTransformationsPluginsRegistry_Clear_Void_Proxy_Api_Network_Direction_Void_Proxy_Api_Network_Operation_"></a> Clear\(Direction, Operation\)

```csharp
void Clear(Direction direction, Operation operation)
```

#### Parameters

`direction` [Direction](Void.Proxy.Api.Network.Direction.md)

`operation` [Operation](Void.Proxy.Api.Network.Operation.md)

### <a id="Void_Minecraft_Network_Registries_Transformations_IMinecraftPacketTransformationsPluginsRegistry_Contains__1_Void_Minecraft_Network_Registries_Transformations_Mappings_TransformationType_"></a> Contains<T\>\(TransformationType\)

```csharp
bool Contains<T>(TransformationType type) where T : IMinecraftPacket
```

#### Parameters

`type` [TransformationType](Void.Minecraft.Network.Registries.Transformations.Mappings.TransformationType.md)

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

#### Type Parameters

`T` 

### <a id="Void_Minecraft_Network_Registries_Transformations_IMinecraftPacketTransformationsPluginsRegistry_Contains_Void_Minecraft_Network_Messages_IMinecraftMessage_Void_Minecraft_Network_Registries_Transformations_Mappings_TransformationType_"></a> Contains\(IMinecraftMessage, TransformationType\)

```csharp
bool Contains(IMinecraftMessage message, TransformationType type)
```

#### Parameters

`message` [IMinecraftMessage](Void.Minecraft.Network.Messages.IMinecraftMessage.md)

`type` [TransformationType](Void.Minecraft.Network.Registries.Transformations.Mappings.TransformationType.md)

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Network_Registries_Transformations_IMinecraftPacketTransformationsPluginsRegistry_Contains_System_Type_Void_Minecraft_Network_Registries_Transformations_Mappings_TransformationType_"></a> Contains\(Type, TransformationType\)

```csharp
bool Contains(Type packetType, TransformationType transformationType)
```

#### Parameters

`packetType` [Type](https://learn.microsoft.com/dotnet/api/system.type)

`transformationType` [TransformationType](Void.Minecraft.Network.Registries.Transformations.Mappings.TransformationType.md)

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Network_Registries_Transformations_IMinecraftPacketTransformationsPluginsRegistry_Get_Void_Proxy_Api_Plugins_IPlugin_"></a> Get\(IPlugin\)

```csharp
IMinecraftPacketTransformationsRegistry Get(IPlugin plugin)
```

#### Parameters

`plugin` [IPlugin](Void.Proxy.Api.Plugins.IPlugin.md)

#### Returns

 [IMinecraftPacketTransformationsRegistry](Void.Minecraft.Network.Registries.Transformations.IMinecraftPacketTransformationsRegistry.md)

### <a id="Void_Minecraft_Network_Registries_Transformations_IMinecraftPacketTransformationsPluginsRegistry_Remove_Void_Proxy_Api_Plugins_IPlugin_"></a> Remove\(IPlugin\)

```csharp
void Remove(IPlugin plugin)
```

#### Parameters

`plugin` [IPlugin](Void.Proxy.Api.Plugins.IPlugin.md)

### <a id="Void_Minecraft_Network_Registries_Transformations_IMinecraftPacketTransformationsPluginsRegistry_Reset"></a> Reset\(\)

```csharp
void Reset()
```

