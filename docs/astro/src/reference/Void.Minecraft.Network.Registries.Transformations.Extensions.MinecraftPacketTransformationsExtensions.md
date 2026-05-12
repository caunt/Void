# <a id="Void_Minecraft_Network_Registries_Transformations_Extensions_MinecraftPacketTransformationsExtensions"></a> Class MinecraftPacketTransformationsExtensions

Namespace: [Void.Minecraft.Network.Registries.Transformations.Extensions](Void.Minecraft.Network.Registries.Transformations.Extensions.md)  
Assembly: Void.Minecraft.dll  

```csharp
public static class MinecraftPacketTransformationsExtensions
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[MinecraftPacketTransformationsExtensions](Void.Minecraft.Network.Registries.Transformations.Extensions.MinecraftPacketTransformationsExtensions.md)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Methods

### <a id="Void_Minecraft_Network_Registries_Transformations_Extensions_MinecraftPacketTransformationsExtensions_RegisterTransformations__1_Void_Minecraft_Network_Registries_Transformations_IMinecraftPacketTransformationsRegistry_Void_Minecraft_Network_ProtocolVersion_System_Collections_Generic_IEnumerable_Void_Minecraft_Network_Registries_Transformations_Mappings_MinecraftPacketTransformationMapping__"></a> RegisterTransformations<T\>\(IMinecraftPacketTransformationsRegistry, ProtocolVersion, params IEnumerable<MinecraftPacketTransformationMapping\>\)

```csharp
public static void RegisterTransformations<T>(this IMinecraftPacketTransformationsRegistry registry, ProtocolVersion protocolVersion, params IEnumerable<MinecraftPacketTransformationMapping> mappings) where T : IMinecraftPacket
```

#### Parameters

`registry` [IMinecraftPacketTransformationsRegistry](Void.Minecraft.Network.Registries.Transformations.IMinecraftPacketTransformationsRegistry.md)

`protocolVersion` [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)

`mappings` [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[MinecraftPacketTransformationMapping](Void.Minecraft.Network.Registries.Transformations.Mappings.MinecraftPacketTransformationMapping.md)\>

#### Type Parameters

`T` 

