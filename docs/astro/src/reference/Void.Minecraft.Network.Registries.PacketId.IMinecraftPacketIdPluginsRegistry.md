# <a id="Void_Minecraft_Network_Registries_PacketId_IMinecraftPacketIdPluginsRegistry"></a> Interface IMinecraftPacketIdPluginsRegistry

Namespace: [Void.Minecraft.Network.Registries.PacketId](Void.Minecraft.Network.Registries.PacketId.md)  
Assembly: Void.Minecraft.dll  

```csharp
public interface IMinecraftPacketIdPluginsRegistry
```

#### Extension Methods

[MinecraftPacketIdPluginsRegistryExtensions.TryGetTransformations\(IMinecraftPacketIdPluginsRegistry, IMinecraftPacketTransformationsPluginsRegistry, IMinecraftPacket, TransformationType, out MinecraftPacketTransformation\[\]\)](Void.Minecraft.Network.Registries.PacketId.Extensions.MinecraftPacketIdPluginsRegistryExtensions.md\#Void\_Minecraft\_Network\_Registries\_PacketId\_Extensions\_MinecraftPacketIdPluginsRegistryExtensions\_TryGetTransformations\_Void\_Minecraft\_Network\_Registries\_PacketId\_IMinecraftPacketIdPluginsRegistry\_Void\_Minecraft\_Network\_Registries\_Transformations\_IMinecraftPacketTransformationsPluginsRegistry\_Void\_Minecraft\_Network\_Messages\_Packets\_IMinecraftPacket\_Void\_Minecraft\_Network\_Registries\_Transformations\_Mappings\_TransformationType\_Void\_Minecraft\_Network\_Registries\_Transformations\_Mappings\_MinecraftPacketTransformation\_\_\_\_), 
[MinecraftPacketIdPluginsRegistryExtensions.TryGetTransformations\(IMinecraftPacketIdPluginsRegistry, IMinecraftPacketTransformationsPluginsRegistry, Type, TransformationType, out MinecraftPacketTransformation\[\]\)](Void.Minecraft.Network.Registries.PacketId.Extensions.MinecraftPacketIdPluginsRegistryExtensions.md\#Void\_Minecraft\_Network\_Registries\_PacketId\_Extensions\_MinecraftPacketIdPluginsRegistryExtensions\_TryGetTransformations\_Void\_Minecraft\_Network\_Registries\_PacketId\_IMinecraftPacketIdPluginsRegistry\_Void\_Minecraft\_Network\_Registries\_Transformations\_IMinecraftPacketTransformationsPluginsRegistry\_System\_Type\_Void\_Minecraft\_Network\_Registries\_Transformations\_Mappings\_TransformationType\_Void\_Minecraft\_Network\_Registries\_Transformations\_Mappings\_MinecraftPacketTransformation\_\_\_\_)

## Properties

### <a id="Void_Minecraft_Network_Registries_PacketId_IMinecraftPacketIdPluginsRegistry_IsEmpty"></a> IsEmpty

```csharp
bool IsEmpty { get; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Network_Registries_PacketId_IMinecraftPacketIdPluginsRegistry_ManagedBy"></a> ManagedBy

```csharp
IPlugin? ManagedBy { get; set; }
```

#### Property Value

 [IPlugin](Void.Proxy.Api.Plugins.IPlugin.md)?

### <a id="Void_Minecraft_Network_Registries_PacketId_IMinecraftPacketIdPluginsRegistry_ProtocolVersion"></a> ProtocolVersion

```csharp
ProtocolVersion? ProtocolVersion { get; set; }
```

#### Property Value

 [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)?

### <a id="Void_Minecraft_Network_Registries_PacketId_IMinecraftPacketIdPluginsRegistry_Read"></a> Read

```csharp
IReadOnlyCollection<IMinecraftPacketIdRegistry> Read { get; }
```

#### Property Value

 [IReadOnlyCollection](https://learn.microsoft.com/dotnet/api/system.collections.generic.ireadonlycollection\-1)<[IMinecraftPacketIdRegistry](Void.Minecraft.Network.Registries.PacketId.IMinecraftPacketIdRegistry.md)\>

### <a id="Void_Minecraft_Network_Registries_PacketId_IMinecraftPacketIdPluginsRegistry_Write"></a> Write

```csharp
IReadOnlyCollection<IMinecraftPacketIdRegistry> Write { get; }
```

#### Property Value

 [IReadOnlyCollection](https://learn.microsoft.com/dotnet/api/system.collections.generic.ireadonlycollection\-1)<[IMinecraftPacketIdRegistry](Void.Minecraft.Network.Registries.PacketId.IMinecraftPacketIdRegistry.md)\>

## Methods

### <a id="Void_Minecraft_Network_Registries_PacketId_IMinecraftPacketIdPluginsRegistry_Clear"></a> Clear\(\)

```csharp
void Clear()
```

### <a id="Void_Minecraft_Network_Registries_PacketId_IMinecraftPacketIdPluginsRegistry_Clear_Void_Proxy_Api_Network_Direction_Void_Proxy_Api_Network_Operation_"></a> Clear\(Direction, Operation\)

```csharp
void Clear(Direction direction, Operation operation)
```

#### Parameters

`direction` [Direction](Void.Proxy.Api.Network.Direction.md)

`operation` [Operation](Void.Proxy.Api.Network.Operation.md)

### <a id="Void_Minecraft_Network_Registries_PacketId_IMinecraftPacketIdPluginsRegistry_Contains__1"></a> Contains<T\>\(\)

```csharp
bool Contains<T>() where T : IMinecraftPacket
```

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

#### Type Parameters

`T` 

### <a id="Void_Minecraft_Network_Registries_PacketId_IMinecraftPacketIdPluginsRegistry_Contains_Void_Proxy_Api_Network_Messages_INetworkMessage_"></a> Contains\(INetworkMessage\)

```csharp
bool Contains(INetworkMessage message)
```

#### Parameters

`message` [INetworkMessage](Void.Proxy.Api.Network.Messages.INetworkMessage.md)

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Network_Registries_PacketId_IMinecraftPacketIdPluginsRegistry_Contains_System_Type_"></a> Contains\(Type\)

```csharp
bool Contains(Type type)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/dotnet/api/system.type)

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Network_Registries_PacketId_IMinecraftPacketIdPluginsRegistry_Get_Void_Proxy_Api_Network_Operation_Void_Proxy_Api_Plugins_IPlugin_"></a> Get\(Operation, IPlugin\)

```csharp
IMinecraftPacketIdRegistry Get(Operation operation, IPlugin plugin)
```

#### Parameters

`operation` [Operation](Void.Proxy.Api.Network.Operation.md)

`plugin` [IPlugin](Void.Proxy.Api.Plugins.IPlugin.md)

#### Returns

 [IMinecraftPacketIdRegistry](Void.Minecraft.Network.Registries.PacketId.IMinecraftPacketIdRegistry.md)

### <a id="Void_Minecraft_Network_Registries_PacketId_IMinecraftPacketIdPluginsRegistry_Remove_Void_Proxy_Api_Plugins_IPlugin_"></a> Remove\(IPlugin\)

```csharp
void Remove(IPlugin plugin)
```

#### Parameters

`plugin` [IPlugin](Void.Proxy.Api.Plugins.IPlugin.md)

### <a id="Void_Minecraft_Network_Registries_PacketId_IMinecraftPacketIdPluginsRegistry_Reset"></a> Reset\(\)

```csharp
void Reset()
```

### <a id="Void_Minecraft_Network_Registries_PacketId_IMinecraftPacketIdPluginsRegistry_TryGetPlugin__1_Void_Proxy_Api_Plugins_IPlugin__"></a> TryGetPlugin<T\>\(out IPlugin\)

```csharp
bool TryGetPlugin<T>(out IPlugin plugin) where T : IMinecraftPacket
```

#### Parameters

`plugin` [IPlugin](Void.Proxy.Api.Plugins.IPlugin.md)

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

#### Type Parameters

`T` 

### <a id="Void_Minecraft_Network_Registries_PacketId_IMinecraftPacketIdPluginsRegistry_TryGetPlugin_Void_Proxy_Api_Network_Messages_INetworkMessage_Void_Proxy_Api_Plugins_IPlugin__"></a> TryGetPlugin\(INetworkMessage, out IPlugin\)

```csharp
bool TryGetPlugin(INetworkMessage message, out IPlugin plugin)
```

#### Parameters

`message` [INetworkMessage](Void.Proxy.Api.Network.Messages.INetworkMessage.md)

`plugin` [IPlugin](Void.Proxy.Api.Plugins.IPlugin.md)

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Network_Registries_PacketId_IMinecraftPacketIdPluginsRegistry_TryGetPlugin_System_Type_Void_Proxy_Api_Plugins_IPlugin__"></a> TryGetPlugin\(Type, out IPlugin\)

```csharp
bool TryGetPlugin(Type type, out IPlugin plugin)
```

#### Parameters

`type` [Type](https://learn.microsoft.com/dotnet/api/system.type)

`plugin` [IPlugin](Void.Proxy.Api.Plugins.IPlugin.md)

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

