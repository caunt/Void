# <a id="Void_Minecraft_Network_Registries_Transformations_Mappings_MinecraftPacketTransformationMapping"></a> Class MinecraftPacketTransformationMapping

Namespace: [Void.Minecraft.Network.Registries.Transformations.Mappings](Void.Minecraft.Network.Registries.Transformations.Mappings.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record MinecraftPacketTransformationMapping : IEquatable<MinecraftPacketTransformationMapping>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[MinecraftPacketTransformationMapping](Void.Minecraft.Network.Registries.Transformations.Mappings.MinecraftPacketTransformationMapping.md)

#### Implements

[IEquatable<MinecraftPacketTransformationMapping\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Network_Registries_Transformations_Mappings_MinecraftPacketTransformationMapping__ctor_Void_Minecraft_Network_ProtocolVersion_Void_Minecraft_Network_ProtocolVersion_Void_Minecraft_Network_Registries_Transformations_Mappings_MinecraftPacketTransformation_"></a> MinecraftPacketTransformationMapping\(ProtocolVersion, ProtocolVersion, MinecraftPacketTransformation\)

```csharp
public MinecraftPacketTransformationMapping(ProtocolVersion From, ProtocolVersion To, MinecraftPacketTransformation Transformation)
```

#### Parameters

`From` [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)

`To` [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)

`Transformation` [MinecraftPacketTransformation](Void.Minecraft.Network.Registries.Transformations.Mappings.MinecraftPacketTransformation.md)

## Properties

### <a id="Void_Minecraft_Network_Registries_Transformations_Mappings_MinecraftPacketTransformationMapping_From"></a> From

```csharp
public ProtocolVersion From { get; init; }
```

#### Property Value

 [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)

### <a id="Void_Minecraft_Network_Registries_Transformations_Mappings_MinecraftPacketTransformationMapping_To"></a> To

```csharp
public ProtocolVersion To { get; init; }
```

#### Property Value

 [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)

### <a id="Void_Minecraft_Network_Registries_Transformations_Mappings_MinecraftPacketTransformationMapping_Transformation"></a> Transformation

```csharp
public MinecraftPacketTransformation Transformation { get; init; }
```

#### Property Value

 [MinecraftPacketTransformation](Void.Minecraft.Network.Registries.Transformations.Mappings.MinecraftPacketTransformation.md)

