# <a id="Void_Minecraft_Network_Registries_IRegistryHolder"></a> Interface IRegistryHolder

Namespace: [Void.Minecraft.Network.Registries](Void.Minecraft.Network.Registries.md)  
Assembly: Void.Minecraft.dll  

```csharp
public interface IRegistryHolder : IDisposable
```

#### Implements

[IDisposable](https://learn.microsoft.com/dotnet/api/system.idisposable)

## Properties

### <a id="Void_Minecraft_Network_Registries_IRegistryHolder_PacketIdPlugins"></a> PacketIdPlugins

```csharp
IMinecraftPacketIdPluginsRegistry PacketIdPlugins { get; }
```

#### Property Value

 [IMinecraftPacketIdPluginsRegistry](Void.Minecraft.Network.Registries.PacketId.IMinecraftPacketIdPluginsRegistry.md)

### <a id="Void_Minecraft_Network_Registries_IRegistryHolder_PacketIdSystem"></a> PacketIdSystem

```csharp
IMinecraftPacketIdSystemRegistry PacketIdSystem { get; }
```

#### Property Value

 [IMinecraftPacketIdSystemRegistry](Void.Minecraft.Network.Registries.PacketId.IMinecraftPacketIdSystemRegistry.md)

### <a id="Void_Minecraft_Network_Registries_IRegistryHolder_PacketTransformationsPlugins"></a> PacketTransformationsPlugins

```csharp
IMinecraftPacketTransformationsPluginsRegistry PacketTransformationsPlugins { get; }
```

#### Property Value

 [IMinecraftPacketTransformationsPluginsRegistry](Void.Minecraft.Network.Registries.Transformations.IMinecraftPacketTransformationsPluginsRegistry.md)

### <a id="Void_Minecraft_Network_Registries_IRegistryHolder_PacketTransformationsSystem"></a> PacketTransformationsSystem

```csharp
IMinecraftPacketTransformationsSystemRegistry PacketTransformationsSystem { get; }
```

#### Property Value

 [IMinecraftPacketTransformationsSystemRegistry](Void.Minecraft.Network.Registries.Transformations.IMinecraftPacketTransformationsSystemRegistry.md)

### <a id="Void_Minecraft_Network_Registries_IRegistryHolder_ProtocolVersion"></a> ProtocolVersion

```csharp
ProtocolVersion ProtocolVersion { get; }
```

#### Property Value

 [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)

## Methods

### <a id="Void_Minecraft_Network_Registries_IRegistryHolder_ClearPlugin_Void_Proxy_Api_Plugins_IPlugin_"></a> ClearPlugin\(IPlugin\)

```csharp
void ClearPlugin(IPlugin plugin)
```

#### Parameters

`plugin` [IPlugin](Void.Proxy.Api.Plugins.IPlugin.md)

### <a id="Void_Minecraft_Network_Registries_IRegistryHolder_ClearPlugins"></a> ClearPlugins\(\)

```csharp
void ClearPlugins()
```

### <a id="Void_Minecraft_Network_Registries_IRegistryHolder_ClearPlugins_Void_Proxy_Api_Network_Direction_Void_Proxy_Api_Network_Operation_"></a> ClearPlugins\(Direction, Operation\)

```csharp
void ClearPlugins(Direction direction, Operation operation)
```

#### Parameters

`direction` [Direction](Void.Proxy.Api.Network.Direction.md)

`operation` [Operation](Void.Proxy.Api.Network.Operation.md)

### <a id="Void_Minecraft_Network_Registries_IRegistryHolder_DisposeBy_Void_Proxy_Api_Plugins_IPlugin_"></a> DisposeBy\(IPlugin\)

```csharp
void DisposeBy(IPlugin managedBy)
```

#### Parameters

`managedBy` [IPlugin](Void.Proxy.Api.Plugins.IPlugin.md)

### <a id="Void_Minecraft_Network_Registries_IRegistryHolder_PrintPackets"></a> PrintPackets\(\)

```csharp
string PrintPackets()
```

#### Returns

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Network_Registries_IRegistryHolder_Setup_Void_Proxy_Api_Plugins_IPlugin_Void_Minecraft_Network_ProtocolVersion_"></a> Setup\(IPlugin, ProtocolVersion\)

```csharp
void Setup(IPlugin managedBy, ProtocolVersion protocolVersion)
```

#### Parameters

`managedBy` [IPlugin](Void.Proxy.Api.Plugins.IPlugin.md)

`protocolVersion` [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)

