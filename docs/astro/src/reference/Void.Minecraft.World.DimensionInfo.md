# <a id="Void_Minecraft_World_DimensionInfo"></a> Class DimensionInfo

Namespace: [Void.Minecraft.World](Void.Minecraft.World.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record DimensionInfo : IEquatable<DimensionInfo>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[DimensionInfo](Void.Minecraft.World.DimensionInfo.md)

#### Implements

[IEquatable<DimensionInfo\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_World_DimensionInfo__ctor_System_String_System_String_System_Boolean_System_Boolean_"></a> DimensionInfo\(string, string?, bool, bool\)

```csharp
public DimensionInfo(string RegistryIdentifier, string? LevelName, bool IsFlat, bool IsDebugType)
```

#### Parameters

`RegistryIdentifier` [string](https://learn.microsoft.com/dotnet/api/system.string)

`LevelName` [string](https://learn.microsoft.com/dotnet/api/system.string)?

`IsFlat` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

`IsDebugType` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

## Properties

### <a id="Void_Minecraft_World_DimensionInfo_IsDebugType"></a> IsDebugType

```csharp
public bool IsDebugType { get; init; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_World_DimensionInfo_IsFlat"></a> IsFlat

```csharp
public bool IsFlat { get; init; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_World_DimensionInfo_LevelName"></a> LevelName

```csharp
public string? LevelName { get; init; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)?

### <a id="Void_Minecraft_World_DimensionInfo_RegistryIdentifier"></a> RegistryIdentifier

```csharp
public string RegistryIdentifier { get; init; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

