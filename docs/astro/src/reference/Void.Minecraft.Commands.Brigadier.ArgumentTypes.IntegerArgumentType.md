# <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_IntegerArgumentType"></a> Class IntegerArgumentType

Namespace: [Void.Minecraft.Commands.Brigadier.ArgumentTypes](Void.Minecraft.Commands.Brigadier.ArgumentTypes.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record IntegerArgumentType : IArgumentType, IAnyArgumentType, IEquatable<IntegerArgumentType>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[IntegerArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IntegerArgumentType.md)

#### Implements

[IArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IArgumentType.md), 
[IAnyArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IAnyArgumentType.md), 
[IEquatable<IntegerArgumentType\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Properties

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_IntegerArgumentType_Examples"></a> Examples

```csharp
public IEnumerable<string> Examples { get; }
```

#### Property Value

 [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[string](https://learn.microsoft.com/dotnet/api/system.string)\>

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_IntegerArgumentType_Maximum"></a> Maximum

```csharp
public required int Maximum { get; init; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_IntegerArgumentType_Minimum"></a> Minimum

```csharp
public required int Minimum { get; init; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

## Methods

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_IntegerArgumentType_GetInteger_Void_Minecraft_Commands_Brigadier_Context_CommandContext_System_String_"></a> GetInteger\(CommandContext, string\)

```csharp
public static int GetInteger(CommandContext context, string name)
```

#### Parameters

`context` [CommandContext](Void.Minecraft.Commands.Brigadier.Context.CommandContext.md)

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)

#### Returns

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_IntegerArgumentType_IntegerArgument"></a> IntegerArgument\(\)

```csharp
public static IntegerArgumentType IntegerArgument()
```

#### Returns

 [IntegerArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IntegerArgumentType.md)

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_IntegerArgumentType_IntegerArgument_System_Int32_"></a> IntegerArgument\(int\)

```csharp
public static IntegerArgumentType IntegerArgument(int min)
```

#### Parameters

`min` [int](https://learn.microsoft.com/dotnet/api/system.int32)

#### Returns

 [IntegerArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IntegerArgumentType.md)

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_IntegerArgumentType_IntegerArgument_System_Int32_System_Int32_"></a> IntegerArgument\(int, int\)

```csharp
public static IntegerArgumentType IntegerArgument(int min, int max)
```

#### Parameters

`min` [int](https://learn.microsoft.com/dotnet/api/system.int32)

`max` [int](https://learn.microsoft.com/dotnet/api/system.int32)

#### Returns

 [IntegerArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IntegerArgumentType.md)

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_IntegerArgumentType_Parse_Void_Minecraft_Commands_Brigadier_StringReader_"></a> Parse\(StringReader\)

```csharp
public IArgumentValue Parse(StringReader reader)
```

#### Parameters

`reader` [StringReader](Void.Minecraft.Commands.Brigadier.StringReader.md)

#### Returns

 [IArgumentValue](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IArgumentValue.md)

