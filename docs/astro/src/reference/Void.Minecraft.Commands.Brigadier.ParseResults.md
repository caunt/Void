# <a id="Void_Minecraft_Commands_Brigadier_ParseResults"></a> Class ParseResults

Namespace: [Void.Minecraft.Commands.Brigadier](Void.Minecraft.Commands.Brigadier.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record ParseResults : IEquatable<ParseResults>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ParseResults](Void.Minecraft.Commands.Brigadier.ParseResults.md)

#### Implements

[IEquatable<ParseResults\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Commands_Brigadier_ParseResults__ctor_Void_Minecraft_Commands_Brigadier_Context_CommandContextBuilder_Void_Minecraft_Commands_Brigadier_IImmutableStringReader_System_Collections_Generic_Dictionary_Void_Minecraft_Commands_Brigadier_Tree_CommandNode_Void_Minecraft_Commands_Brigadier_Exceptions_CommandSyntaxException__"></a> ParseResults\(CommandContextBuilder, IImmutableStringReader, Dictionary<CommandNode, CommandSyntaxException\>\)

```csharp
public ParseResults(CommandContextBuilder Context, IImmutableStringReader Reader, Dictionary<CommandNode, CommandSyntaxException> Exceptions)
```

#### Parameters

`Context` [CommandContextBuilder](Void.Minecraft.Commands.Brigadier.Context.CommandContextBuilder.md)

`Reader` [IImmutableStringReader](Void.Minecraft.Commands.Brigadier.IImmutableStringReader.md)

`Exceptions` [Dictionary](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary\-2)<[CommandNode](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md), [CommandSyntaxException](Void.Minecraft.Commands.Brigadier.Exceptions.CommandSyntaxException.md)\>

### <a id="Void_Minecraft_Commands_Brigadier_ParseResults__ctor_Void_Minecraft_Commands_Brigadier_Context_CommandContextBuilder_"></a> ParseResults\(CommandContextBuilder\)

```csharp
public ParseResults(CommandContextBuilder context)
```

#### Parameters

`context` [CommandContextBuilder](Void.Minecraft.Commands.Brigadier.Context.CommandContextBuilder.md)

## Properties

### <a id="Void_Minecraft_Commands_Brigadier_ParseResults_Context"></a> Context

```csharp
public CommandContextBuilder Context { get; init; }
```

#### Property Value

 [CommandContextBuilder](Void.Minecraft.Commands.Brigadier.Context.CommandContextBuilder.md)

### <a id="Void_Minecraft_Commands_Brigadier_ParseResults_Exceptions"></a> Exceptions

```csharp
public Dictionary<CommandNode, CommandSyntaxException> Exceptions { get; init; }
```

#### Property Value

 [Dictionary](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary\-2)<[CommandNode](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md), [CommandSyntaxException](Void.Minecraft.Commands.Brigadier.Exceptions.CommandSyntaxException.md)\>

### <a id="Void_Minecraft_Commands_Brigadier_ParseResults_Reader"></a> Reader

```csharp
public IImmutableStringReader Reader { get; init; }
```

#### Property Value

 [IImmutableStringReader](Void.Minecraft.Commands.Brigadier.IImmutableStringReader.md)

