# <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_FloatArgumentType"></a> Class FloatArgumentType

Namespace: [Void.Minecraft.Commands.Brigadier.ArgumentTypes](Void.Minecraft.Commands.Brigadier.ArgumentTypes.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record FloatArgumentType : IArgumentType, IAnyArgumentType, IEquatable<FloatArgumentType>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[FloatArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.FloatArgumentType.md)

#### Implements

[IArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IArgumentType.md), 
[IAnyArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IAnyArgumentType.md), 
[IEquatable<FloatArgumentType\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Properties

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_FloatArgumentType_Examples"></a> Examples

```csharp
public IEnumerable<string> Examples { get; }
```

#### Property Value

 [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[string](https://learn.microsoft.com/dotnet/api/system.string)\>

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_FloatArgumentType_Maximum"></a> Maximum

```csharp
public required float Maximum { get; init; }
```

#### Property Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_FloatArgumentType_Minimum"></a> Minimum

```csharp
public required float Minimum { get; init; }
```

#### Property Value

 [float](https://learn.microsoft.com/dotnet/api/system.single)

## Methods

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_FloatArgumentType_FloatArgument"></a> FloatArgument\(\)

```csharp
public static FloatArgumentType FloatArgument()
```

#### Returns

 [FloatArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.FloatArgumentType.md)

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_FloatArgumentType_FloatArgument_System_Single_"></a> FloatArgument\(float\)

```csharp
public static FloatArgumentType FloatArgument(float min)
```

#### Parameters

`min` [float](https://learn.microsoft.com/dotnet/api/system.single)

#### Returns

 [FloatArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.FloatArgumentType.md)

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_FloatArgumentType_FloatArgument_System_Single_System_Single_"></a> FloatArgument\(float, float\)

```csharp
public static FloatArgumentType FloatArgument(float min, float max)
```

#### Parameters

`min` [float](https://learn.microsoft.com/dotnet/api/system.single)

`max` [float](https://learn.microsoft.com/dotnet/api/system.single)

#### Returns

 [FloatArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.FloatArgumentType.md)

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_FloatArgumentType_GetFloat_Void_Minecraft_Commands_Brigadier_Context_CommandContext_System_String_"></a> GetFloat\(CommandContext, string\)

```csharp
public static float GetFloat(CommandContext context, string name)
```

#### Parameters

`context` [CommandContext](Void.Minecraft.Commands.Brigadier.Context.CommandContext.md)

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)

#### Returns

 [float](https://learn.microsoft.com/dotnet/api/system.single)

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_FloatArgumentType_Parse_Void_Minecraft_Commands_Brigadier_StringReader_"></a> Parse\(StringReader\)

```csharp
public IArgumentValue Parse(StringReader reader)
```

#### Parameters

`reader` [StringReader](Void.Minecraft.Commands.Brigadier.StringReader.md)

#### Returns

 [IArgumentValue](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IArgumentValue.md)

