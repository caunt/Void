# <a id="Void_Minecraft_Network_Registries_PacketId_Extensions_MinecraftPacketIdRegistryExtensions"></a> Class MinecraftPacketIdRegistryExtensions

Namespace: [Void.Minecraft.Network.Registries.PacketId.Extensions](Void.Minecraft.Network.Registries.PacketId.Extensions.md)  
Assembly: Void.Minecraft.dll  

```csharp
public static class MinecraftPacketIdRegistryExtensions
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[MinecraftPacketIdRegistryExtensions](Void.Minecraft.Network.Registries.PacketId.Extensions.MinecraftPacketIdRegistryExtensions.md)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Methods

### <a id="Void_Minecraft_Network_Registries_PacketId_Extensions_MinecraftPacketIdRegistryExtensions_RegisterPacket__1_Void_Minecraft_Network_Registries_PacketId_IMinecraftPacketIdRegistry_Void_Minecraft_Network_ProtocolVersion_Void_Minecraft_Network_Registries_PacketId_Mappings_MinecraftPacketIdMapping___"></a> RegisterPacket<T\>\(IMinecraftPacketIdRegistry, ProtocolVersion, params MinecraftPacketIdMapping\[\]\)

```csharp
public static void RegisterPacket<T>(this IMinecraftPacketIdRegistry registry, ProtocolVersion protocolVersion, params MinecraftPacketIdMapping[] mappings) where T : IMinecraftPacket
```

#### Parameters

`registry` [IMinecraftPacketIdRegistry](Void.Minecraft.Network.Registries.PacketId.IMinecraftPacketIdRegistry.md)

`protocolVersion` [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)

`mappings` [MinecraftPacketIdMapping](Void.Minecraft.Network.Registries.PacketId.Mappings.MinecraftPacketIdMapping.md)\[\]

#### Type Parameters

`T` 

