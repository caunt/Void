# <a id="Void_Minecraft_Commands_Brigadier_Context_ParsedArgument"></a> Class ParsedArgument

Namespace: [Void.Minecraft.Commands.Brigadier.Context](Void.Minecraft.Commands.Brigadier.Context.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record ParsedArgument : IParsedArgument, IEquatable<ParsedArgument>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ParsedArgument](Void.Minecraft.Commands.Brigadier.Context.ParsedArgument.md)

#### Implements

[IParsedArgument](Void.Minecraft.Commands.Brigadier.Context.IParsedArgument.md), 
[IEquatable<ParsedArgument\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Commands_Brigadier_Context_ParsedArgument__ctor_System_Int32_System_Int32_Void_Minecraft_Commands_Brigadier_ArgumentTypes_IArgumentValue_"></a> ParsedArgument\(int, int, IArgumentValue\)

```csharp
public ParsedArgument(int Start, int End, IArgumentValue Result)
```

#### Parameters

`Start` [int](https://learn.microsoft.com/dotnet/api/system.int32)

`End` [int](https://learn.microsoft.com/dotnet/api/system.int32)

`Result` [IArgumentValue](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IArgumentValue.md)

## Properties

### <a id="Void_Minecraft_Commands_Brigadier_Context_ParsedArgument_End"></a> End

```csharp
public int End { get; init; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

### <a id="Void_Minecraft_Commands_Brigadier_Context_ParsedArgument_Range"></a> Range

```csharp
public StringRange Range { get; }
```

#### Property Value

 [StringRange](Void.Minecraft.Commands.Brigadier.Context.StringRange.md)

### <a id="Void_Minecraft_Commands_Brigadier_Context_ParsedArgument_Result"></a> Result

```csharp
public IArgumentValue Result { get; init; }
```

#### Property Value

 [IArgumentValue](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IArgumentValue.md)

### <a id="Void_Minecraft_Commands_Brigadier_Context_ParsedArgument_Start"></a> Start

```csharp
public int Start { get; init; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

