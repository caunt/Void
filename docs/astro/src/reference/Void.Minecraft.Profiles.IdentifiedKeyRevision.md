# <a id="Void_Minecraft_Profiles_IdentifiedKeyRevision"></a> Class IdentifiedKeyRevision

Namespace: [Void.Minecraft.Profiles](Void.Minecraft.Profiles.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record IdentifiedKeyRevision : IEquatable<IdentifiedKeyRevision>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[IdentifiedKeyRevision](Void.Minecraft.Profiles.IdentifiedKeyRevision.md)

#### Implements

[IEquatable<IdentifiedKeyRevision\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Profiles_IdentifiedKeyRevision__ctor_System_Collections_Generic_IEnumerable_Void_Minecraft_Profiles_IdentifiedKeyRevision__System_Collections_Generic_List_Void_Minecraft_Network_ProtocolVersion__"></a> IdentifiedKeyRevision\(IEnumerable<IdentifiedKeyRevision\>, List<ProtocolVersion\>\)

```csharp
public IdentifiedKeyRevision(IEnumerable<IdentifiedKeyRevision> BackwardsCompatibleTo, List<ProtocolVersion> ApplicableTo)
```

#### Parameters

`BackwardsCompatibleTo` [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[IdentifiedKeyRevision](Void.Minecraft.Profiles.IdentifiedKeyRevision.md)\>

`ApplicableTo` [List](https://learn.microsoft.com/dotnet/api/system.collections.generic.list\-1)<[ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)\>

## Fields

### <a id="Void_Minecraft_Profiles_IdentifiedKeyRevision_GenericV1Revision"></a> GenericV1Revision

```csharp
public static readonly IdentifiedKeyRevision GenericV1Revision
```

#### Field Value

 [IdentifiedKeyRevision](Void.Minecraft.Profiles.IdentifiedKeyRevision.md)

### <a id="Void_Minecraft_Profiles_IdentifiedKeyRevision_LinkedV2Revision"></a> LinkedV2Revision

```csharp
public static readonly IdentifiedKeyRevision LinkedV2Revision
```

#### Field Value

 [IdentifiedKeyRevision](Void.Minecraft.Profiles.IdentifiedKeyRevision.md)

## Properties

### <a id="Void_Minecraft_Profiles_IdentifiedKeyRevision_ApplicableTo"></a> ApplicableTo

```csharp
public List<ProtocolVersion> ApplicableTo { get; init; }
```

#### Property Value

 [List](https://learn.microsoft.com/dotnet/api/system.collections.generic.list\-1)<[ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)\>

### <a id="Void_Minecraft_Profiles_IdentifiedKeyRevision_BackwardsCompatibleTo"></a> BackwardsCompatibleTo

```csharp
public IEnumerable<IdentifiedKeyRevision> BackwardsCompatibleTo { get; init; }
```

#### Property Value

 [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[IdentifiedKeyRevision](Void.Minecraft.Profiles.IdentifiedKeyRevision.md)\>

