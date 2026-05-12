# <a id="Void_Minecraft_Commands_Brigadier_Context_CommandContext"></a> Class CommandContext

Namespace: [Void.Minecraft.Commands.Brigadier.Context](Void.Minecraft.Commands.Brigadier.Context.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record CommandContext : IEquatable<CommandContext>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[CommandContext](Void.Minecraft.Commands.Brigadier.Context.CommandContext.md)

#### Implements

[IEquatable<CommandContext\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Commands_Brigadier_Context_CommandContext__ctor_Void_Proxy_Api_Commands_ICommandSource_System_String_System_Collections_Generic_Dictionary_System_String_Void_Minecraft_Commands_Brigadier_Context_IParsedArgument__Void_Minecraft_Commands_Brigadier_CommandExecutor_Void_Minecraft_Commands_Brigadier_Tree_CommandNode_System_Collections_Generic_List_Void_Minecraft_Commands_Brigadier_Context_ParsedCommandNode__Void_Minecraft_Commands_Brigadier_Context_StringRange_Void_Minecraft_Commands_Brigadier_Context_CommandContext_Void_Minecraft_Commands_Brigadier_RedirectModifier_System_Boolean_"></a> CommandContext\(ICommandSource, string, Dictionary<string, IParsedArgument\>, CommandExecutor?, CommandNode, List<ParsedCommandNode\>, StringRange, CommandContext?, RedirectModifier?, bool\)

```csharp
public CommandContext(ICommandSource Source, string Input, Dictionary<string, IParsedArgument> Arguments, CommandExecutor? Executor, CommandNode RootNode, List<ParsedCommandNode> Nodes, StringRange Range, CommandContext? Child, RedirectModifier? RedirectModifier, bool Forks)
```

#### Parameters

`Source` [ICommandSource](Void.Proxy.Api.Commands.ICommandSource.md)

`Input` [string](https://learn.microsoft.com/dotnet/api/system.string)

`Arguments` [Dictionary](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary\-2)<[string](https://learn.microsoft.com/dotnet/api/system.string), [IParsedArgument](Void.Minecraft.Commands.Brigadier.Context.IParsedArgument.md)\>

`Executor` [CommandExecutor](Void.Minecraft.Commands.Brigadier.CommandExecutor.md)?

`RootNode` [CommandNode](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md)

`Nodes` [List](https://learn.microsoft.com/dotnet/api/system.collections.generic.list\-1)<[ParsedCommandNode](Void.Minecraft.Commands.Brigadier.Context.ParsedCommandNode.md)\>

`Range` [StringRange](Void.Minecraft.Commands.Brigadier.Context.StringRange.md)

`Child` [CommandContext](Void.Minecraft.Commands.Brigadier.Context.CommandContext.md)?

`RedirectModifier` [RedirectModifier](Void.Minecraft.Commands.Brigadier.RedirectModifier.md)?

`Forks` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

## Properties

### <a id="Void_Minecraft_Commands_Brigadier_Context_CommandContext_Arguments"></a> Arguments

```csharp
public Dictionary<string, IParsedArgument> Arguments { get; init; }
```

#### Property Value

 [Dictionary](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary\-2)<[string](https://learn.microsoft.com/dotnet/api/system.string), [IParsedArgument](Void.Minecraft.Commands.Brigadier.Context.IParsedArgument.md)\>

### <a id="Void_Minecraft_Commands_Brigadier_Context_CommandContext_Child"></a> Child

```csharp
public CommandContext? Child { get; init; }
```

#### Property Value

 [CommandContext](Void.Minecraft.Commands.Brigadier.Context.CommandContext.md)?

### <a id="Void_Minecraft_Commands_Brigadier_Context_CommandContext_Executor"></a> Executor

```csharp
public CommandExecutor? Executor { get; init; }
```

#### Property Value

 [CommandExecutor](Void.Minecraft.Commands.Brigadier.CommandExecutor.md)?

### <a id="Void_Minecraft_Commands_Brigadier_Context_CommandContext_Forks"></a> Forks

```csharp
public bool Forks { get; init; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Commands_Brigadier_Context_CommandContext_HasNodes"></a> HasNodes

```csharp
public bool HasNodes { get; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Commands_Brigadier_Context_CommandContext_Input"></a> Input

```csharp
public string Input { get; init; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Commands_Brigadier_Context_CommandContext_Nodes"></a> Nodes

```csharp
public List<ParsedCommandNode> Nodes { get; init; }
```

#### Property Value

 [List](https://learn.microsoft.com/dotnet/api/system.collections.generic.list\-1)<[ParsedCommandNode](Void.Minecraft.Commands.Brigadier.Context.ParsedCommandNode.md)\>

### <a id="Void_Minecraft_Commands_Brigadier_Context_CommandContext_Range"></a> Range

```csharp
public StringRange Range { get; init; }
```

#### Property Value

 [StringRange](Void.Minecraft.Commands.Brigadier.Context.StringRange.md)

### <a id="Void_Minecraft_Commands_Brigadier_Context_CommandContext_RedirectModifier"></a> RedirectModifier

```csharp
public RedirectModifier? RedirectModifier { get; init; }
```

#### Property Value

 [RedirectModifier](Void.Minecraft.Commands.Brigadier.RedirectModifier.md)?

### <a id="Void_Minecraft_Commands_Brigadier_Context_CommandContext_RootNode"></a> RootNode

```csharp
public CommandNode RootNode { get; init; }
```

#### Property Value

 [CommandNode](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md)

### <a id="Void_Minecraft_Commands_Brigadier_Context_CommandContext_Source"></a> Source

```csharp
public ICommandSource Source { get; init; }
```

#### Property Value

 [ICommandSource](Void.Proxy.Api.Commands.ICommandSource.md)

## Methods

### <a id="Void_Minecraft_Commands_Brigadier_Context_CommandContext_CopyFor_Void_Proxy_Api_Commands_ICommandSource_"></a> CopyFor\(ICommandSource\)

```csharp
public CommandContext CopyFor(ICommandSource source)
```

#### Parameters

`source` [ICommandSource](Void.Proxy.Api.Commands.ICommandSource.md)

#### Returns

 [CommandContext](Void.Minecraft.Commands.Brigadier.Context.CommandContext.md)

### <a id="Void_Minecraft_Commands_Brigadier_Context_CommandContext_GetArgument__1_System_String_"></a> GetArgument<TType\>\(string\)

```csharp
public TType GetArgument<TType>(string name)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)

#### Returns

 TType

#### Type Parameters

`TType` 

### <a id="Void_Minecraft_Commands_Brigadier_Context_CommandContext_GetLastChild"></a> GetLastChild\(\)

```csharp
public CommandContext GetLastChild()
```

#### Returns

 [CommandContext](Void.Minecraft.Commands.Brigadier.Context.CommandContext.md)

### <a id="Void_Minecraft_Commands_Brigadier_Context_CommandContext_TryGetArgument__1_System_String___0__"></a> TryGetArgument<TType\>\(string, out TType\)

```csharp
public bool TryGetArgument<TType>(string name, out TType type)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)

`type` TType

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

#### Type Parameters

`TType` 

