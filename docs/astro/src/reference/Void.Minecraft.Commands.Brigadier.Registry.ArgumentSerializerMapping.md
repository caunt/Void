# <a id="Void_Minecraft_Commands_Brigadier_Registry_ArgumentSerializerMapping"></a> Class ArgumentSerializerMapping

Namespace: [Void.Minecraft.Commands.Brigadier.Registry](Void.Minecraft.Commands.Brigadier.Registry.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record ArgumentSerializerMapping : IEquatable<ArgumentSerializerMapping>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ArgumentSerializerMapping](Void.Minecraft.Commands.Brigadier.Registry.ArgumentSerializerMapping.md)

#### Implements

[IEquatable<ArgumentSerializerMapping\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Commands_Brigadier_Registry_ArgumentSerializerMapping__ctor_System_String_System_Collections_Generic_Dictionary_Void_Minecraft_Network_ProtocolVersion_System_Int32__"></a> ArgumentSerializerMapping\(string, Dictionary<ProtocolVersion, int\>\)

```csharp
public ArgumentSerializerMapping(string Identifier, Dictionary<ProtocolVersion, int> VersionParserMappings)
```

#### Parameters

`Identifier` [string](https://learn.microsoft.com/dotnet/api/system.string)

`VersionParserMappings` [Dictionary](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary\-2)<[ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md), [int](https://learn.microsoft.com/dotnet/api/system.int32)\>

### <a id="Void_Minecraft_Commands_Brigadier_Registry_ArgumentSerializerMapping__ctor_System_String_"></a> ArgumentSerializerMapping\(string\)

```csharp
public ArgumentSerializerMapping(string identifier)
```

#### Parameters

`identifier` [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Commands_Brigadier_Registry_ArgumentSerializerMapping__ctor_System_String_Void_Minecraft_Network_ProtocolVersion_System_Int32_"></a> ArgumentSerializerMapping\(string, ProtocolVersion, int\)

```csharp
public ArgumentSerializerMapping(string identifier, ProtocolVersion protocolVersion, int parserId)
```

#### Parameters

`identifier` [string](https://learn.microsoft.com/dotnet/api/system.string)

`protocolVersion` [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)

`parserId` [int](https://learn.microsoft.com/dotnet/api/system.int32)

## Properties

### <a id="Void_Minecraft_Commands_Brigadier_Registry_ArgumentSerializerMapping_Identifier"></a> Identifier

```csharp
public string Identifier { get; init; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Commands_Brigadier_Registry_ArgumentSerializerMapping_VersionParserIdMapping"></a> VersionParserIdMapping

```csharp
public FrozenDictionary<ProtocolVersion, int> VersionParserIdMapping { get; }
```

#### Property Value

 [FrozenDictionary](https://learn.microsoft.com/dotnet/api/system.collections.frozen.frozendictionary\-2)<[ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md), [int](https://learn.microsoft.com/dotnet/api/system.int32)\>

### <a id="Void_Minecraft_Commands_Brigadier_Registry_ArgumentSerializerMapping_VersionParserMappings"></a> VersionParserMappings

```csharp
public Dictionary<ProtocolVersion, int> VersionParserMappings { get; init; }
```

#### Property Value

 [Dictionary](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary\-2)<[ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md), [int](https://learn.microsoft.com/dotnet/api/system.int32)\>

## Methods

### <a id="Void_Minecraft_Commands_Brigadier_Registry_ArgumentSerializerMapping_TryGetParserId_Void_Minecraft_Network_ProtocolVersion_System_Int32__"></a> TryGetParserId\(ProtocolVersion, out int\)

```csharp
public bool TryGetParserId(ProtocolVersion version, out int id)
```

#### Parameters

`version` [ProtocolVersion](Void.Minecraft.Network.ProtocolVersion.md)

`id` [int](https://learn.microsoft.com/dotnet/api/system.int32)

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

