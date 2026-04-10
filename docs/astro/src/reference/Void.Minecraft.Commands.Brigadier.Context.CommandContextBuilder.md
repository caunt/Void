# <a id="Void_Minecraft_Commands_Brigadier_Context_CommandContextBuilder"></a> Class CommandContextBuilder

Namespace: [Void.Minecraft.Commands.Brigadier.Context](Void.Minecraft.Commands.Brigadier.Context.md)  
Assembly: Void.Minecraft.dll  

```csharp
public class CommandContextBuilder
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[CommandContextBuilder](Void.Minecraft.Commands.Brigadier.Context.CommandContextBuilder.md)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Commands_Brigadier_Context_CommandContextBuilder__ctor_Void_Minecraft_Commands_Brigadier_CommandDispatcher_Void_Proxy_Api_Commands_ICommandSource_Void_Minecraft_Commands_Brigadier_Tree_CommandNode_System_Int32_"></a> CommandContextBuilder\(CommandDispatcher, ICommandSource, CommandNode, int\)

```csharp
public CommandContextBuilder(CommandDispatcher dispatcher, ICommandSource source, CommandNode rootNode, int start)
```

#### Parameters

`dispatcher` [CommandDispatcher](Void.Minecraft.Commands.Brigadier.CommandDispatcher.md)

`source` [ICommandSource](Void.Proxy.Api.Commands.ICommandSource.md)

`rootNode` [CommandNode](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md)

`start` [int](https://learn.microsoft.com/dotnet/api/system.int32)

## Properties

### <a id="Void_Minecraft_Commands_Brigadier_Context_CommandContextBuilder_Arguments"></a> Arguments

```csharp
public Dictionary<string, IParsedArgument> Arguments { get; }
```

#### Property Value

 [Dictionary](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary\-2)<[string](https://learn.microsoft.com/dotnet/api/system.string), [IParsedArgument](Void.Minecraft.Commands.Brigadier.Context.IParsedArgument.md)\>

### <a id="Void_Minecraft_Commands_Brigadier_Context_CommandContextBuilder_Child"></a> Child

```csharp
public CommandContextBuilder? Child { get; set; }
```

#### Property Value

 [CommandContextBuilder](Void.Minecraft.Commands.Brigadier.Context.CommandContextBuilder.md)?

### <a id="Void_Minecraft_Commands_Brigadier_Context_CommandContextBuilder_Command"></a> Command

```csharp
public CommandExecutor? Command { get; set; }
```

#### Property Value

 [CommandExecutor](Void.Minecraft.Commands.Brigadier.CommandExecutor.md)?

### <a id="Void_Minecraft_Commands_Brigadier_Context_CommandContextBuilder_Dispatcher"></a> Dispatcher

```csharp
public CommandDispatcher Dispatcher { get; set; }
```

#### Property Value

 [CommandDispatcher](Void.Minecraft.Commands.Brigadier.CommandDispatcher.md)

### <a id="Void_Minecraft_Commands_Brigadier_Context_CommandContextBuilder_IsFork"></a> IsFork

```csharp
public bool IsFork { get; set; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Commands_Brigadier_Context_CommandContextBuilder_Nodes"></a> Nodes

```csharp
public List<ParsedCommandNode> Nodes { get; }
```

#### Property Value

 [List](https://learn.microsoft.com/dotnet/api/system.collections.generic.list\-1)<[ParsedCommandNode](Void.Minecraft.Commands.Brigadier.Context.ParsedCommandNode.md)\>

### <a id="Void_Minecraft_Commands_Brigadier_Context_CommandContextBuilder_Range"></a> Range

```csharp
public StringRange Range { get; set; }
```

#### Property Value

 [StringRange](Void.Minecraft.Commands.Brigadier.Context.StringRange.md)

### <a id="Void_Minecraft_Commands_Brigadier_Context_CommandContextBuilder_RedirectModifier"></a> RedirectModifier

```csharp
public RedirectModifier? RedirectModifier { get; set; }
```

#### Property Value

 [RedirectModifier](Void.Minecraft.Commands.Brigadier.RedirectModifier.md)?

### <a id="Void_Minecraft_Commands_Brigadier_Context_CommandContextBuilder_RootNode"></a> RootNode

```csharp
public CommandNode RootNode { get; set; }
```

#### Property Value

 [CommandNode](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md)

### <a id="Void_Minecraft_Commands_Brigadier_Context_CommandContextBuilder_Source"></a> Source

```csharp
public ICommandSource Source { get; set; }
```

#### Property Value

 [ICommandSource](Void.Proxy.Api.Commands.ICommandSource.md)

### <a id="Void_Minecraft_Commands_Brigadier_Context_CommandContextBuilder_Start"></a> Start

```csharp
public int Start { get; set; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

## Methods

### <a id="Void_Minecraft_Commands_Brigadier_Context_CommandContextBuilder_Build_System_String_"></a> Build\(string\)

```csharp
public CommandContext Build(string input)
```

#### Parameters

`input` [string](https://learn.microsoft.com/dotnet/api/system.string)

#### Returns

 [CommandContext](Void.Minecraft.Commands.Brigadier.Context.CommandContext.md)

### <a id="Void_Minecraft_Commands_Brigadier_Context_CommandContextBuilder_BuildSuggestions_System_Int32_"></a> BuildSuggestions\(int\)

```csharp
public SuggestionContext BuildSuggestions(int cursor)
```

#### Parameters

`cursor` [int](https://learn.microsoft.com/dotnet/api/system.int32)

#### Returns

 [SuggestionContext](Void.Minecraft.Commands.Brigadier.Context.SuggestionContext.md)

### <a id="Void_Minecraft_Commands_Brigadier_Context_CommandContextBuilder_Copy"></a> Copy\(\)

```csharp
public CommandContextBuilder Copy()
```

#### Returns

 [CommandContextBuilder](Void.Minecraft.Commands.Brigadier.Context.CommandContextBuilder.md)

### <a id="Void_Minecraft_Commands_Brigadier_Context_CommandContextBuilder_GetLastChild"></a> GetLastChild\(\)

```csharp
public CommandContextBuilder GetLastChild()
```

#### Returns

 [CommandContextBuilder](Void.Minecraft.Commands.Brigadier.Context.CommandContextBuilder.md)

### <a id="Void_Minecraft_Commands_Brigadier_Context_CommandContextBuilder_WithArgument_System_String_Void_Minecraft_Commands_Brigadier_Context_ParsedArgument_"></a> WithArgument\(string, ParsedArgument\)

```csharp
public CommandContextBuilder WithArgument(string name, ParsedArgument argument)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)

`argument` [ParsedArgument](Void.Minecraft.Commands.Brigadier.Context.ParsedArgument.md)

#### Returns

 [CommandContextBuilder](Void.Minecraft.Commands.Brigadier.Context.CommandContextBuilder.md)

### <a id="Void_Minecraft_Commands_Brigadier_Context_CommandContextBuilder_WithChild_Void_Minecraft_Commands_Brigadier_Context_CommandContextBuilder_"></a> WithChild\(CommandContextBuilder\)

```csharp
public CommandContextBuilder WithChild(CommandContextBuilder child)
```

#### Parameters

`child` [CommandContextBuilder](Void.Minecraft.Commands.Brigadier.Context.CommandContextBuilder.md)

#### Returns

 [CommandContextBuilder](Void.Minecraft.Commands.Brigadier.Context.CommandContextBuilder.md)

### <a id="Void_Minecraft_Commands_Brigadier_Context_CommandContextBuilder_WithExecutor_Void_Minecraft_Commands_Brigadier_CommandExecutor_"></a> WithExecutor\(CommandExecutor?\)

```csharp
public CommandContextBuilder WithExecutor(CommandExecutor? command)
```

#### Parameters

`command` [CommandExecutor](Void.Minecraft.Commands.Brigadier.CommandExecutor.md)?

#### Returns

 [CommandContextBuilder](Void.Minecraft.Commands.Brigadier.Context.CommandContextBuilder.md)

### <a id="Void_Minecraft_Commands_Brigadier_Context_CommandContextBuilder_WithNode_Void_Minecraft_Commands_Brigadier_Tree_CommandNode_Void_Minecraft_Commands_Brigadier_Context_StringRange_"></a> WithNode\(CommandNode, StringRange\)

```csharp
public CommandContextBuilder WithNode(CommandNode node, StringRange range)
```

#### Parameters

`node` [CommandNode](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md)

`range` [StringRange](Void.Minecraft.Commands.Brigadier.Context.StringRange.md)

#### Returns

 [CommandContextBuilder](Void.Minecraft.Commands.Brigadier.Context.CommandContextBuilder.md)

### <a id="Void_Minecraft_Commands_Brigadier_Context_CommandContextBuilder_WithSource_Void_Proxy_Api_Commands_ICommandSource_"></a> WithSource\(ICommandSource\)

```csharp
public CommandContextBuilder WithSource(ICommandSource source)
```

#### Parameters

`source` [ICommandSource](Void.Proxy.Api.Commands.ICommandSource.md)

#### Returns

 [CommandContextBuilder](Void.Minecraft.Commands.Brigadier.Context.CommandContextBuilder.md)

