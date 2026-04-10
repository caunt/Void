# <a id="Void_Minecraft_Network_Registries_PacketId_Extensions_MinecraftPacketIdPluginsRegistryExtensions"></a> Class MinecraftPacketIdPluginsRegistryExtensions

Namespace: [Void.Minecraft.Network.Registries.PacketId.Extensions](Void.Minecraft.Network.Registries.PacketId.Extensions.md)  
Assembly: Void.Minecraft.dll  

```csharp
public static class MinecraftPacketIdPluginsRegistryExtensions
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[MinecraftPacketIdPluginsRegistryExtensions](Void.Minecraft.Network.Registries.PacketId.Extensions.MinecraftPacketIdPluginsRegistryExtensions.md)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Methods

### <a id="Void_Minecraft_Network_Registries_PacketId_Extensions_MinecraftPacketIdPluginsRegistryExtensions_TryGetTransformations_Void_Minecraft_Network_Registries_PacketId_IMinecraftPacketIdPluginsRegistry_Void_Minecraft_Network_Registries_Transformations_IMinecraftPacketTransformationsPluginsRegistry_Void_Minecraft_Network_Messages_Packets_IMinecraftPacket_Void_Minecraft_Network_Registries_Transformations_Mappings_TransformationType_Void_Minecraft_Network_Registries_Transformations_Mappings_MinecraftPacketTransformation____"></a> TryGetTransformations\(IMinecraftPacketIdPluginsRegistry, IMinecraftPacketTransformationsPluginsRegistry, IMinecraftPacket, TransformationType, out MinecraftPacketTransformation\[\]\)

```csharp
public static bool TryGetTransformations(this IMinecraftPacketIdPluginsRegistry registriesHolder, IMinecraftPacketTransformationsPluginsRegistry transformationsHolder, IMinecraftPacket packet, TransformationType transformationType, out MinecraftPacketTransformation[] transformations)
```

#### Parameters

`registriesHolder` [IMinecraftPacketIdPluginsRegistry](Void.Minecraft.Network.Registries.PacketId.IMinecraftPacketIdPluginsRegistry.md)

`transformationsHolder` [IMinecraftPacketTransformationsPluginsRegistry](Void.Minecraft.Network.Registries.Transformations.IMinecraftPacketTransformationsPluginsRegistry.md)

`packet` [IMinecraftPacket](Void.Minecraft.Network.Messages.Packets.IMinecraftPacket.md)

`transformationType` [TransformationType](Void.Minecraft.Network.Registries.Transformations.Mappings.TransformationType.md)

`transformations` [MinecraftPacketTransformation](Void.Minecraft.Network.Registries.Transformations.Mappings.MinecraftPacketTransformation.md)\[\]

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Network_Registries_PacketId_Extensions_MinecraftPacketIdPluginsRegistryExtensions_TryGetTransformations_Void_Minecraft_Network_Registries_PacketId_IMinecraftPacketIdPluginsRegistry_Void_Minecraft_Network_Registries_Transformations_IMinecraftPacketTransformationsPluginsRegistry_System_Type_Void_Minecraft_Network_Registries_Transformations_Mappings_TransformationType_Void_Minecraft_Network_Registries_Transformations_Mappings_MinecraftPacketTransformation____"></a> TryGetTransformations\(IMinecraftPacketIdPluginsRegistry, IMinecraftPacketTransformationsPluginsRegistry, Type, TransformationType, out MinecraftPacketTransformation\[\]\)

```csharp
public static bool TryGetTransformations(this IMinecraftPacketIdPluginsRegistry registriesHolder, IMinecraftPacketTransformationsPluginsRegistry transformationsHolder, Type packetType, TransformationType transformationType, out MinecraftPacketTransformation[] transformations)
```

#### Parameters

`registriesHolder` [IMinecraftPacketIdPluginsRegistry](Void.Minecraft.Network.Registries.PacketId.IMinecraftPacketIdPluginsRegistry.md)

`transformationsHolder` [IMinecraftPacketTransformationsPluginsRegistry](Void.Minecraft.Network.Registries.Transformations.IMinecraftPacketTransformationsPluginsRegistry.md)

`packetType` [Type](https://learn.microsoft.com/dotnet/api/system.type)

`transformationType` [TransformationType](Void.Minecraft.Network.Registries.Transformations.Mappings.TransformationType.md)

`transformations` [MinecraftPacketTransformation](Void.Minecraft.Network.Registries.Transformations.Mappings.MinecraftPacketTransformation.md)\[\]

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

