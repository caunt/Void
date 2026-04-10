# <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_StringArgumentType"></a> Class StringArgumentType

Namespace: [Void.Minecraft.Commands.Brigadier.ArgumentTypes](Void.Minecraft.Commands.Brigadier.ArgumentTypes.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record StringArgumentType : IArgumentType, IAnyArgumentType, IEquatable<StringArgumentType>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[StringArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.StringArgumentType.md)

#### Implements

[IArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IArgumentType.md), 
[IAnyArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IAnyArgumentType.md), 
[IEquatable<StringArgumentType\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Properties

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_StringArgumentType_Examples"></a> Examples

```csharp
public IEnumerable<string> Examples { get; }
```

#### Property Value

 [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[string](https://learn.microsoft.com/dotnet/api/system.string)\>

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_StringArgumentType_Type"></a> Type

```csharp
public StringArgumentType.StringType Type { get; init; }
```

#### Property Value

 [StringArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.StringArgumentType.md).[StringType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.StringArgumentType.StringType.md)

## Methods

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_StringArgumentType_GreedyString"></a> GreedyString\(\)

```csharp
public static StringArgumentType GreedyString()
```

#### Returns

 [StringArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.StringArgumentType.md)

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_StringArgumentType_Parse_Void_Minecraft_Commands_Brigadier_StringReader_"></a> Parse\(StringReader\)

```csharp
public IArgumentValue Parse(StringReader reader)
```

#### Parameters

`reader` [StringReader](Void.Minecraft.Commands.Brigadier.StringReader.md)

#### Returns

 [IArgumentValue](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IArgumentValue.md)

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_StringArgumentType_String"></a> String\(\)

```csharp
public static StringArgumentType String()
```

#### Returns

 [StringArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.StringArgumentType.md)

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_StringArgumentType_Word"></a> Word\(\)

```csharp
public static StringArgumentType Word()
```

#### Returns

 [StringArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.StringArgumentType.md)

