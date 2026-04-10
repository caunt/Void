# <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_RegistryKey_RegistryKeyArgumentType"></a> Class RegistryKeyArgumentType

Namespace: [Void.Minecraft.Commands.Brigadier.ArgumentTypes.RegistryKey](Void.Minecraft.Commands.Brigadier.ArgumentTypes.RegistryKey.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record RegistryKeyArgumentType : IArgumentType, IAnyArgumentType, IEquatable<RegistryKeyArgumentType>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[RegistryKeyArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.RegistryKey.RegistryKeyArgumentType.md)

#### Derived

[ResourceKeyArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.RegistryKey.ResourceKeyArgumentType.md), 
[ResourceOrTagArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.RegistryKey.ResourceOrTagArgumentType.md), 
[ResourceOrTagKeyArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.RegistryKey.ResourceOrTagKeyArgumentType.md), 
[ResourceSelectorArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.RegistryKey.ResourceSelectorArgumentType.md)

#### Implements

[IArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IArgumentType.md), 
[IAnyArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IAnyArgumentType.md), 
[IEquatable<RegistryKeyArgumentType\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_RegistryKey_RegistryKeyArgumentType__ctor_System_String_"></a> RegistryKeyArgumentType\(string\)

```csharp
public RegistryKeyArgumentType(string Identifier)
```

#### Parameters

`Identifier` [string](https://learn.microsoft.com/dotnet/api/system.string)

## Properties

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_RegistryKey_RegistryKeyArgumentType_Examples"></a> Examples

```csharp
public IEnumerable<string> Examples { get; }
```

#### Property Value

 [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[string](https://learn.microsoft.com/dotnet/api/system.string)\>

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_RegistryKey_RegistryKeyArgumentType_Identifier"></a> Identifier

```csharp
public string Identifier { get; init; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

## Methods

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_RegistryKey_RegistryKeyArgumentType_Parse_Void_Minecraft_Commands_Brigadier_StringReader_"></a> Parse\(StringReader\)

```csharp
public IArgumentValue Parse(StringReader reader)
```

#### Parameters

`reader` [StringReader](Void.Minecraft.Commands.Brigadier.StringReader.md)

#### Returns

 [IArgumentValue](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IArgumentValue.md)

