# <a id="Void_Minecraft_Commands_Brigadier_CommandDispatcher"></a> Class CommandDispatcher

Namespace: [Void.Minecraft.Commands.Brigadier](Void.Minecraft.Commands.Brigadier.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record CommandDispatcher : ICommandDispatcher, IEquatable<CommandDispatcher>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[CommandDispatcher](Void.Minecraft.Commands.Brigadier.CommandDispatcher.md)

#### Implements

[ICommandDispatcher](Void.Proxy.Api.Commands.ICommandDispatcher.md), 
[IEquatable<CommandDispatcher\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Commands_Brigadier_CommandDispatcher__ctor_Void_Minecraft_Commands_Brigadier_Tree_Nodes_RootCommandNode_"></a> CommandDispatcher\(RootCommandNode\)

```csharp
public CommandDispatcher(RootCommandNode Root)
```

#### Parameters

`Root` [RootCommandNode](Void.Minecraft.Commands.Brigadier.Tree.Nodes.RootCommandNode.md)

### <a id="Void_Minecraft_Commands_Brigadier_CommandDispatcher__ctor"></a> CommandDispatcher\(\)

```csharp
public CommandDispatcher()
```

## Fields

### <a id="Void_Minecraft_Commands_Brigadier_CommandDispatcher_ArgumentSeparator"></a> ArgumentSeparator

```csharp
public const char ArgumentSeparator = ' '
```

#### Field Value

 [char](https://learn.microsoft.com/dotnet/api/system.char)

## Properties

### <a id="Void_Minecraft_Commands_Brigadier_CommandDispatcher_Consumer"></a> Consumer

```csharp
public ResultConsumer Consumer { get; set; }
```

#### Property Value

 [ResultConsumer](Void.Minecraft.Commands.Brigadier.ResultConsumer.md)

### <a id="Void_Minecraft_Commands_Brigadier_CommandDispatcher_Root"></a> Root

```csharp
public RootCommandNode Root { get; init; }
```

#### Property Value

 [RootCommandNode](Void.Minecraft.Commands.Brigadier.Tree.Nodes.RootCommandNode.md)

## Methods

### <a id="Void_Minecraft_Commands_Brigadier_CommandDispatcher_Add_Void_Proxy_Api_Commands_ICommandNode_"></a> Add\(ICommandNode\)

Registers <code class="paramref">node</code> as a direct child of the root command node,
making it available for dispatch and tab-completion.

```csharp
public void Add(ICommandNode node)
```

#### Parameters

`node` [ICommandNode](Void.Proxy.Api.Commands.ICommandNode.md)

The command node to add. Must be a concrete <code>CommandNode</code> instance;
any other implementation throws <xref href="System.ArgumentException" data-throw-if-not-resolved="false"></xref>.

### <a id="Void_Minecraft_Commands_Brigadier_CommandDispatcher_All_Void_Minecraft_Commands_Brigadier_Tree_CommandNode_"></a> All\(CommandNode?\)

```csharp
public IEnumerable<CommandNode> All(CommandNode? root = null)
```

#### Parameters

`root` [CommandNode](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md)?

#### Returns

 [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[CommandNode](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md)\>

### <a id="Void_Minecraft_Commands_Brigadier_CommandDispatcher_ExecuteAsync_System_String_Void_Proxy_Api_Commands_ICommandSource_System_Threading_CancellationToken_"></a> ExecuteAsync\(string, ICommandSource, CancellationToken\)

```csharp
public ValueTask<int> ExecuteAsync(string input, ICommandSource source, CancellationToken cancellationToken)
```

#### Parameters

`input` [string](https://learn.microsoft.com/dotnet/api/system.string)

`source` [ICommandSource](Void.Proxy.Api.Commands.ICommandSource.md)

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask\-1)<[int](https://learn.microsoft.com/dotnet/api/system.int32)\>

### <a id="Void_Minecraft_Commands_Brigadier_CommandDispatcher_ExecuteAsync_Void_Minecraft_Commands_Brigadier_StringReader_Void_Proxy_Api_Commands_ICommandSource_System_Threading_CancellationToken_"></a> ExecuteAsync\(StringReader, ICommandSource, CancellationToken\)

```csharp
public ValueTask<int> ExecuteAsync(StringReader input, ICommandSource source, CancellationToken cancellationToken)
```

#### Parameters

`input` [StringReader](Void.Minecraft.Commands.Brigadier.StringReader.md)

`source` [ICommandSource](Void.Proxy.Api.Commands.ICommandSource.md)

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask\-1)<[int](https://learn.microsoft.com/dotnet/api/system.int32)\>

### <a id="Void_Minecraft_Commands_Brigadier_CommandDispatcher_ExecuteAsync_Void_Minecraft_Commands_Brigadier_ParseResults_System_Threading_CancellationToken_"></a> ExecuteAsync\(ParseResults, CancellationToken\)

```csharp
public ValueTask<int> ExecuteAsync(ParseResults parse, CancellationToken cancellationToken)
```

#### Parameters

`parse` [ParseResults](Void.Minecraft.Commands.Brigadier.ParseResults.md)

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask\-1)<[int](https://learn.microsoft.com/dotnet/api/system.int32)\>

### <a id="Void_Minecraft_Commands_Brigadier_CommandDispatcher_FindNode_System_Collections_Generic_List_System_String__"></a> FindNode\(List<string\>\)

```csharp
public CommandNode? FindNode(List<string> path)
```

#### Parameters

`path` [List](https://learn.microsoft.com/dotnet/api/system.collections.generic.list\-1)<[string](https://learn.microsoft.com/dotnet/api/system.string)\>

#### Returns

 [CommandNode](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md)?

### <a id="Void_Minecraft_Commands_Brigadier_CommandDispatcher_GetAllUsageAsync_Void_Minecraft_Commands_Brigadier_Tree_CommandNode_Void_Proxy_Api_Commands_ICommandSource_System_Boolean_System_Threading_CancellationToken_"></a> GetAllUsageAsync\(CommandNode, ICommandSource, bool, CancellationToken\)

```csharp
public ValueTask<string[]> GetAllUsageAsync(CommandNode node, ICommandSource source, bool restricted, CancellationToken cancellationToken)
```

#### Parameters

`node` [CommandNode](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md)

`source` [ICommandSource](Void.Proxy.Api.Commands.ICommandSource.md)

`restricted` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask\-1)<[string](https://learn.microsoft.com/dotnet/api/system.string)\[\]\>

### <a id="Void_Minecraft_Commands_Brigadier_CommandDispatcher_GetCompletionSuggestions_Void_Minecraft_Commands_Brigadier_ParseResults_System_Threading_CancellationToken_"></a> GetCompletionSuggestions\(ParseResults, CancellationToken\)

```csharp
public static ValueTask<Suggestions> GetCompletionSuggestions(ParseResults parse, CancellationToken cancellationToken)
```

#### Parameters

`parse` [ParseResults](Void.Minecraft.Commands.Brigadier.ParseResults.md)

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask\-1)<[Suggestions](Void.Minecraft.Commands.Brigadier.Suggestion.Suggestions.md)\>

### <a id="Void_Minecraft_Commands_Brigadier_CommandDispatcher_GetCompletionSuggestions_Void_Minecraft_Commands_Brigadier_ParseResults_System_Int32_System_Threading_CancellationToken_"></a> GetCompletionSuggestions\(ParseResults, int, CancellationToken\)

```csharp
public static ValueTask<Suggestions> GetCompletionSuggestions(ParseResults parse, int cursor, CancellationToken cancellationToken)
```

#### Parameters

`parse` [ParseResults](Void.Minecraft.Commands.Brigadier.ParseResults.md)

`cursor` [int](https://learn.microsoft.com/dotnet/api/system.int32)

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask\-1)<[Suggestions](Void.Minecraft.Commands.Brigadier.Suggestion.Suggestions.md)\>

### <a id="Void_Minecraft_Commands_Brigadier_CommandDispatcher_GetPath_Void_Minecraft_Commands_Brigadier_Tree_CommandNode_"></a> GetPath\(CommandNode\)

```csharp
public List<string> GetPath(CommandNode target)
```

#### Parameters

`target` [CommandNode](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md)

#### Returns

 [List](https://learn.microsoft.com/dotnet/api/system.collections.generic.list\-1)<[string](https://learn.microsoft.com/dotnet/api/system.string)\>

### <a id="Void_Minecraft_Commands_Brigadier_CommandDispatcher_GetSmartUsageAsync_Void_Minecraft_Commands_Brigadier_Tree_CommandNode_Void_Proxy_Api_Commands_ICommandSource_System_Threading_CancellationToken_"></a> GetSmartUsageAsync\(CommandNode, ICommandSource, CancellationToken\)

```csharp
public ValueTask<Dictionary<CommandNode, string>> GetSmartUsageAsync(CommandNode node, ICommandSource source, CancellationToken cancellationToken)
```

#### Parameters

`node` [CommandNode](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md)

`source` [ICommandSource](Void.Proxy.Api.Commands.ICommandSource.md)

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask\-1)<[Dictionary](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary\-2)<[CommandNode](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md), [string](https://learn.microsoft.com/dotnet/api/system.string)\>\>

### <a id="Void_Minecraft_Commands_Brigadier_CommandDispatcher_Parse_System_String_Void_Proxy_Api_Commands_ICommandSource_System_Threading_CancellationToken_"></a> Parse\(string, ICommandSource, CancellationToken\)

```csharp
public ValueTask<ParseResults> Parse(string command, ICommandSource source, CancellationToken cancellationToken)
```

#### Parameters

`command` [string](https://learn.microsoft.com/dotnet/api/system.string)

`source` [ICommandSource](Void.Proxy.Api.Commands.ICommandSource.md)

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask\-1)<[ParseResults](Void.Minecraft.Commands.Brigadier.ParseResults.md)\>

### <a id="Void_Minecraft_Commands_Brigadier_CommandDispatcher_ParseAsync_Void_Minecraft_Commands_Brigadier_StringReader_Void_Proxy_Api_Commands_ICommandSource_System_Threading_CancellationToken_"></a> ParseAsync\(StringReader, ICommandSource, CancellationToken\)

```csharp
public ValueTask<ParseResults> ParseAsync(StringReader command, ICommandSource source, CancellationToken cancellationToken)
```

#### Parameters

`command` [StringReader](Void.Minecraft.Commands.Brigadier.StringReader.md)

`source` [ICommandSource](Void.Proxy.Api.Commands.ICommandSource.md)

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask\-1)<[ParseResults](Void.Minecraft.Commands.Brigadier.ParseResults.md)\>

### <a id="Void_Minecraft_Commands_Brigadier_CommandDispatcher_Register_Void_Minecraft_Commands_Brigadier_Builder_LiteralArgumentBuilder_"></a> Register\(LiteralArgumentBuilder\)

```csharp
public LiteralCommandNode Register(LiteralArgumentBuilder command)
```

#### Parameters

`command` [LiteralArgumentBuilder](Void.Minecraft.Commands.Brigadier.Builder.LiteralArgumentBuilder.md)

#### Returns

 [LiteralCommandNode](Void.Minecraft.Commands.Brigadier.Tree.Nodes.LiteralCommandNode.md)

### <a id="Void_Minecraft_Commands_Brigadier_CommandDispatcher_Register_System_Func_Void_Minecraft_Commands_Brigadier_IArgumentContext_Void_Minecraft_Commands_Brigadier_Builder_LiteralArgumentBuilder__"></a> Register\(Func<IArgumentContext, LiteralArgumentBuilder\>\)

```csharp
public LiteralCommandNode Register(Func<IArgumentContext, LiteralArgumentBuilder> command)
```

#### Parameters

`command` [Func](https://learn.microsoft.com/dotnet/api/system.func\-2)<[IArgumentContext](Void.Minecraft.Commands.Brigadier.IArgumentContext.md), [LiteralArgumentBuilder](Void.Minecraft.Commands.Brigadier.Builder.LiteralArgumentBuilder.md)\>

#### Returns

 [LiteralCommandNode](Void.Minecraft.Commands.Brigadier.Tree.Nodes.LiteralCommandNode.md)

### <a id="Void_Minecraft_Commands_Brigadier_CommandDispatcher_SuggestAsync_System_String_Void_Proxy_Api_Commands_ICommandSource_System_Threading_CancellationToken_"></a> SuggestAsync\(string, ICommandSource, CancellationToken\)

```csharp
public ValueTask<Suggestions> SuggestAsync(string input, ICommandSource source, CancellationToken cancellationToken)
```

#### Parameters

`input` [string](https://learn.microsoft.com/dotnet/api/system.string)

`source` [ICommandSource](Void.Proxy.Api.Commands.ICommandSource.md)

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask\-1)<[Suggestions](Void.Minecraft.Commands.Brigadier.Suggestion.Suggestions.md)\>

### <a id="Void_Minecraft_Commands_Brigadier_CommandDispatcher_SuggestAsync_Void_Minecraft_Commands_Brigadier_StringReader_Void_Proxy_Api_Commands_ICommandSource_System_Threading_CancellationToken_"></a> SuggestAsync\(StringReader, ICommandSource, CancellationToken\)

```csharp
public ValueTask<Suggestions> SuggestAsync(StringReader input, ICommandSource source, CancellationToken cancellationToken)
```

#### Parameters

`input` [StringReader](Void.Minecraft.Commands.Brigadier.StringReader.md)

`source` [ICommandSource](Void.Proxy.Api.Commands.ICommandSource.md)

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask\-1)<[Suggestions](Void.Minecraft.Commands.Brigadier.Suggestion.Suggestions.md)\>

