# <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_LongArgumentType"></a> Class LongArgumentType

Namespace: [Void.Minecraft.Commands.Brigadier.ArgumentTypes](Void.Minecraft.Commands.Brigadier.ArgumentTypes.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record LongArgumentType : IArgumentType, IAnyArgumentType, IEquatable<LongArgumentType>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[LongArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.LongArgumentType.md)

#### Implements

[IArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IArgumentType.md), 
[IAnyArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IAnyArgumentType.md), 
[IEquatable<LongArgumentType\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Properties

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_LongArgumentType_Examples"></a> Examples

```csharp
public IEnumerable<string> Examples { get; }
```

#### Property Value

 [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[string](https://learn.microsoft.com/dotnet/api/system.string)\>

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_LongArgumentType_Maximum"></a> Maximum

```csharp
public required long Maximum { get; init; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_LongArgumentType_Minimum"></a> Minimum

```csharp
public required long Minimum { get; init; }
```

#### Property Value

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

## Methods

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_LongArgumentType_GetLong_Void_Minecraft_Commands_Brigadier_Context_CommandContext_System_String_"></a> GetLong\(CommandContext, string\)

```csharp
public static long GetLong(CommandContext context, string name)
```

#### Parameters

`context` [CommandContext](Void.Minecraft.Commands.Brigadier.Context.CommandContext.md)

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)

#### Returns

 [long](https://learn.microsoft.com/dotnet/api/system.int64)

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_LongArgumentType_LongArgument"></a> LongArgument\(\)

```csharp
public static LongArgumentType LongArgument()
```

#### Returns

 [LongArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.LongArgumentType.md)

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_LongArgumentType_LongArgument_System_Int64_"></a> LongArgument\(long\)

```csharp
public static LongArgumentType LongArgument(long min)
```

#### Parameters

`min` [long](https://learn.microsoft.com/dotnet/api/system.int64)

#### Returns

 [LongArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.LongArgumentType.md)

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_LongArgumentType_LongArgument_System_Int64_System_Int64_"></a> LongArgument\(long, long\)

```csharp
public static LongArgumentType LongArgument(long min, long max)
```

#### Parameters

`min` [long](https://learn.microsoft.com/dotnet/api/system.int64)

`max` [long](https://learn.microsoft.com/dotnet/api/system.int64)

#### Returns

 [LongArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.LongArgumentType.md)

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_LongArgumentType_Parse_Void_Minecraft_Commands_Brigadier_StringReader_"></a> Parse\(StringReader\)

```csharp
public IArgumentValue Parse(StringReader reader)
```

#### Parameters

`reader` [StringReader](Void.Minecraft.Commands.Brigadier.StringReader.md)

#### Returns

 [IArgumentValue](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IArgumentValue.md)

