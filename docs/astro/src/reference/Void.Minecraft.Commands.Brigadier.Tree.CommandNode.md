# <a id="Void_Minecraft_Commands_Brigadier_Tree_CommandNode"></a> Class CommandNode

Namespace: [Void.Minecraft.Commands.Brigadier.Tree](Void.Minecraft.Commands.Brigadier.Tree.md)  
Assembly: Void.Minecraft.dll  

```csharp
public abstract class CommandNode : ICommandNode
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[CommandNode](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md)

#### Derived

[ArgumentCommandNode](Void.Minecraft.Commands.Brigadier.Tree.Nodes.ArgumentCommandNode.md), 
[LiteralCommandNode](Void.Minecraft.Commands.Brigadier.Tree.Nodes.LiteralCommandNode.md), 
[RootCommandNode](Void.Minecraft.Commands.Brigadier.Tree.Nodes.RootCommandNode.md)

#### Implements

[ICommandNode](Void.Proxy.Api.Commands.ICommandNode.md)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Commands_Brigadier_Tree_CommandNode__ctor_Void_Minecraft_Commands_Brigadier_CommandExecutor_Void_Minecraft_Commands_Brigadier_Tree_CommandRequirement_Void_Minecraft_Commands_Brigadier_Tree_CommandNode_Void_Minecraft_Commands_Brigadier_RedirectModifier_System_Boolean_"></a> CommandNode\(CommandExecutor?, CommandRequirement?, CommandNode?, RedirectModifier?, bool\)

```csharp
protected CommandNode(CommandExecutor? executor = null, CommandRequirement? requirement = null, CommandNode? redirectTarget = null, RedirectModifier? redirectModifier = null, bool isForks = false)
```

#### Parameters

`executor` [CommandExecutor](Void.Minecraft.Commands.Brigadier.CommandExecutor.md)?

`requirement` [CommandRequirement](Void.Minecraft.Commands.Brigadier.Tree.CommandRequirement.md)?

`redirectTarget` [CommandNode](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md)?

`redirectModifier` [RedirectModifier](Void.Minecraft.Commands.Brigadier.RedirectModifier.md)?

`isForks` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

## Properties

### <a id="Void_Minecraft_Commands_Brigadier_Tree_CommandNode_Children"></a> Children

```csharp
public IEnumerable<CommandNode> Children { get; }
```

#### Property Value

 [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[CommandNode](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md)\>

### <a id="Void_Minecraft_Commands_Brigadier_Tree_CommandNode_Examples"></a> Examples

```csharp
public abstract IEnumerable<string> Examples { get; }
```

#### Property Value

 [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[string](https://learn.microsoft.com/dotnet/api/system.string)\>

### <a id="Void_Minecraft_Commands_Brigadier_Tree_CommandNode_Executor"></a> Executor

```csharp
public CommandExecutor? Executor { get; set; }
```

#### Property Value

 [CommandExecutor](Void.Minecraft.Commands.Brigadier.CommandExecutor.md)?

### <a id="Void_Minecraft_Commands_Brigadier_Tree_CommandNode_IsForks"></a> IsForks

```csharp
public bool IsForks { get; set; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Commands_Brigadier_Tree_CommandNode_Name"></a> Name

```csharp
public abstract string Name { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Commands_Brigadier_Tree_CommandNode_RedirectModifier"></a> RedirectModifier

```csharp
public RedirectModifier? RedirectModifier { get; set; }
```

#### Property Value

 [RedirectModifier](Void.Minecraft.Commands.Brigadier.RedirectModifier.md)?

### <a id="Void_Minecraft_Commands_Brigadier_Tree_CommandNode_RedirectTarget"></a> RedirectTarget

```csharp
public CommandNode? RedirectTarget { get; set; }
```

#### Property Value

 [CommandNode](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md)?

### <a id="Void_Minecraft_Commands_Brigadier_Tree_CommandNode_Requirement"></a> Requirement

```csharp
public CommandRequirement? Requirement { get; set; }
```

#### Property Value

 [CommandRequirement](Void.Minecraft.Commands.Brigadier.Tree.CommandRequirement.md)?

### <a id="Void_Minecraft_Commands_Brigadier_Tree_CommandNode_SortedKey"></a> SortedKey

```csharp
protected abstract string SortedKey { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Commands_Brigadier_Tree_CommandNode_UsageText"></a> UsageText

```csharp
public abstract string UsageText { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

## Methods

### <a id="Void_Minecraft_Commands_Brigadier_Tree_CommandNode_AddChild_Void_Minecraft_Commands_Brigadier_Tree_CommandNode_"></a> AddChild\(CommandNode\)

```csharp
public void AddChild(CommandNode node)
```

#### Parameters

`node` [CommandNode](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md)

### <a id="Void_Minecraft_Commands_Brigadier_Tree_CommandNode_CanUseAsync_Void_Proxy_Api_Commands_ICommandSource_System_Threading_CancellationToken_"></a> CanUseAsync\(ICommandSource, CancellationToken\)

```csharp
public ValueTask<bool> CanUseAsync(ICommandSource source, CancellationToken cancellationToken)
```

#### Parameters

`source` [ICommandSource](Void.Proxy.Api.Commands.ICommandSource.md)

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask\-1)<[bool](https://learn.microsoft.com/dotnet/api/system.boolean)\>

### <a id="Void_Minecraft_Commands_Brigadier_Tree_CommandNode_CreateBuilder"></a> CreateBuilder\(\)

```csharp
public abstract IArgumentBuilder<CommandNode> CreateBuilder()
```

#### Returns

 [IArgumentBuilder](Void.Minecraft.Commands.Brigadier.Builder.IArgumentBuilder\-1.md)<[CommandNode](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md)\>

### <a id="Void_Minecraft_Commands_Brigadier_Tree_CommandNode_FindAmbiguities_Void_Minecraft_Commands_Brigadier_AmbiguousConsumer_"></a> FindAmbiguities\(AmbiguousConsumer\)

```csharp
public void FindAmbiguities(AmbiguousConsumer consumer)
```

#### Parameters

`consumer` [AmbiguousConsumer](Void.Minecraft.Commands.Brigadier.AmbiguousConsumer.md)

### <a id="Void_Minecraft_Commands_Brigadier_Tree_CommandNode_GetChild_System_String_"></a> GetChild\(string\)

```csharp
public CommandNode GetChild(string name)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)

#### Returns

 [CommandNode](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md)

### <a id="Void_Minecraft_Commands_Brigadier_Tree_CommandNode_GetRelevantNodes_Void_Minecraft_Commands_Brigadier_StringReader_"></a> GetRelevantNodes\(StringReader\)

```csharp
public IEnumerable<CommandNode> GetRelevantNodes(StringReader reader)
```

#### Parameters

`reader` [StringReader](Void.Minecraft.Commands.Brigadier.StringReader.md)

#### Returns

 [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[CommandNode](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md)\>

### <a id="Void_Minecraft_Commands_Brigadier_Tree_CommandNode_IsValidInput_System_String_"></a> IsValidInput\(string\)

```csharp
public abstract bool IsValidInput(string input)
```

#### Parameters

`input` [string](https://learn.microsoft.com/dotnet/api/system.string)

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Commands_Brigadier_Tree_CommandNode_ListSuggestionsAsync_Void_Minecraft_Commands_Brigadier_Context_CommandContext_Void_Minecraft_Commands_Brigadier_Suggestion_SuggestionsBuilder_System_Threading_CancellationToken_"></a> ListSuggestionsAsync\(CommandContext, SuggestionsBuilder, CancellationToken\)

```csharp
public abstract ValueTask<Suggestions> ListSuggestionsAsync(CommandContext context, SuggestionsBuilder builder, CancellationToken cancellationToken)
```

#### Parameters

`context` [CommandContext](Void.Minecraft.Commands.Brigadier.Context.CommandContext.md)

`builder` [SuggestionsBuilder](Void.Minecraft.Commands.Brigadier.Suggestion.SuggestionsBuilder.md)

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask\-1)<[Suggestions](Void.Minecraft.Commands.Brigadier.Suggestion.Suggestions.md)\>

### <a id="Void_Minecraft_Commands_Brigadier_Tree_CommandNode_Parse_Void_Minecraft_Commands_Brigadier_StringReader_Void_Minecraft_Commands_Brigadier_Context_CommandContextBuilder_"></a> Parse\(StringReader, CommandContextBuilder\)

```csharp
public abstract void Parse(StringReader reader, CommandContextBuilder context)
```

#### Parameters

`reader` [StringReader](Void.Minecraft.Commands.Brigadier.StringReader.md)

`context` [CommandContextBuilder](Void.Minecraft.Commands.Brigadier.Context.CommandContextBuilder.md)

