# <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_DoubleArgumentType"></a> Class DoubleArgumentType

Namespace: [Void.Minecraft.Commands.Brigadier.ArgumentTypes](Void.Minecraft.Commands.Brigadier.ArgumentTypes.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record DoubleArgumentType : IArgumentType, IAnyArgumentType, IEquatable<DoubleArgumentType>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[DoubleArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.DoubleArgumentType.md)

#### Implements

[IArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IArgumentType.md), 
[IAnyArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IAnyArgumentType.md), 
[IEquatable<DoubleArgumentType\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Properties

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_DoubleArgumentType_Examples"></a> Examples

```csharp
public IEnumerable<string> Examples { get; }
```

#### Property Value

 [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[string](https://learn.microsoft.com/dotnet/api/system.string)\>

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_DoubleArgumentType_Maximum"></a> Maximum

```csharp
public required double Maximum { get; init; }
```

#### Property Value

 [double](https://learn.microsoft.com/dotnet/api/system.double)

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_DoubleArgumentType_Minimum"></a> Minimum

```csharp
public required double Minimum { get; init; }
```

#### Property Value

 [double](https://learn.microsoft.com/dotnet/api/system.double)

## Methods

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_DoubleArgumentType_DoubleArgument"></a> DoubleArgument\(\)

```csharp
public static DoubleArgumentType DoubleArgument()
```

#### Returns

 [DoubleArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.DoubleArgumentType.md)

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_DoubleArgumentType_DoubleArgument_System_Double_"></a> DoubleArgument\(double\)

```csharp
public static DoubleArgumentType DoubleArgument(double min)
```

#### Parameters

`min` [double](https://learn.microsoft.com/dotnet/api/system.double)

#### Returns

 [DoubleArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.DoubleArgumentType.md)

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_DoubleArgumentType_DoubleArgument_System_Double_System_Double_"></a> DoubleArgument\(double, double\)

```csharp
public static DoubleArgumentType DoubleArgument(double min, double max)
```

#### Parameters

`min` [double](https://learn.microsoft.com/dotnet/api/system.double)

`max` [double](https://learn.microsoft.com/dotnet/api/system.double)

#### Returns

 [DoubleArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.DoubleArgumentType.md)

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_DoubleArgumentType_GetDouble_Void_Minecraft_Commands_Brigadier_Context_CommandContext_System_String_"></a> GetDouble\(CommandContext, string\)

```csharp
public static double GetDouble(CommandContext context, string name)
```

#### Parameters

`context` [CommandContext](Void.Minecraft.Commands.Brigadier.Context.CommandContext.md)

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)

#### Returns

 [double](https://learn.microsoft.com/dotnet/api/system.double)

### <a id="Void_Minecraft_Commands_Brigadier_ArgumentTypes_DoubleArgumentType_Parse_Void_Minecraft_Commands_Brigadier_StringReader_"></a> Parse\(StringReader\)

```csharp
public IArgumentValue Parse(StringReader reader)
```

#### Parameters

`reader` [StringReader](Void.Minecraft.Commands.Brigadier.StringReader.md)

#### Returns

 [IArgumentValue](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IArgumentValue.md)

