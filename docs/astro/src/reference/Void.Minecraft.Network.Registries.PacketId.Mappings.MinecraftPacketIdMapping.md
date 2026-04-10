# <a id="Void_Minecraft_Network_Registries_PacketId_Mappings_MinecraftPacketIdMapping"></a> Class MinecraftPacketIdMapping

Namespace: [Void.Minecraft.Network.Registries.PacketId.Mappings](Void.Minecraft.Network.Registries.PacketId.Mappings.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record MinecraftPacketIdMapping : IEquatable<MinecraftPacketIdMapping>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[MinecraftPacketIdMapping](Void.Minecraft.Network.Registries.PacketId.Mappings.MinecraftPacketIdMapping.md)

#### Implements

[IEquatable<MinecraftPacketIdMapping\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Network_Registries_PacketId_Mappings_MinecraftPacketIdMapping__ctor_System_Int32_Void_Minecraft_Network_ProtocolVersion_Void_Minecraft_Network_ProtocolVersion_"></a> MinecraftPacketIdMapping\(int, ProtocolVersion, ProtocolVersion?\)

```csharp
public MinecraftPacketIdMapping(int Id, ProtocolVersion ProtocolVersion, ProtocolVersion? LastValidProtocolVersion = null)
```

#### Parameters

`Id` [int](https://learn.microsoft.com/dotnet/api/system.int32)

`ProtocolVersion` [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)

`LastValidProtocolVersion` [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)?

## Properties

### <a id="Void_Minecraft_Network_Registries_PacketId_Mappings_MinecraftPacketIdMapping_Id"></a> Id

```csharp
public int Id { get; init; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Void_Minecraft_Network_Registries_PacketId_Mappings_MinecraftPacketIdMapping_LastValidProtocolVersion"></a> LastValidProtocolVersion

```csharp
public ProtocolVersion? LastValidProtocolVersion { get; init; }
```

#### Property Value

 [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)?

### <a id="Void_Minecraft_Network_Registries_PacketId_Mappings_MinecraftPacketIdMapping_ProtocolVersion"></a> ProtocolVersion

```csharp
public ProtocolVersion ProtocolVersion { get; init; }
```

#### Property Value

 [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)

