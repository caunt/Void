# <a id="Void_Minecraft_Commands_Brigadier_Tree_Nodes_RootCommandNode"></a> Class RootCommandNode

Namespace: [Void.Minecraft.Commands.Brigadier.Tree.Nodes](Void.Minecraft.Commands.Brigadier.Tree.Nodes.md)  
Assembly: Void.Minecraft.dll  

```csharp
public class RootCommandNode : CommandNode, ICommandNode
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[CommandNode](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md) ← 
[RootCommandNode](Void.Minecraft.Commands.Brigadier.Tree.Nodes.RootCommandNode.md)

#### Implements

[ICommandNode](Void.Proxy.Api.Commands.ICommandNode.md)

#### Inherited Members

[CommandNode.IsForks](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md\#Void\_Minecraft\_Commands\_Brigadier\_Tree\_CommandNode\_IsForks), 
[CommandNode.Requirement](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md\#Void\_Minecraft\_Commands\_Brigadier\_Tree\_CommandNode\_Requirement), 
[CommandNode.Executor](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md\#Void\_Minecraft\_Commands\_Brigadier\_Tree\_CommandNode\_Executor), 
[CommandNode.RedirectTarget](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md\#Void\_Minecraft\_Commands\_Brigadier\_Tree\_CommandNode\_RedirectTarget), 
[CommandNode.RedirectModifier](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md\#Void\_Minecraft\_Commands\_Brigadier\_Tree\_CommandNode\_RedirectModifier), 
[CommandNode.Children](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md\#Void\_Minecraft\_Commands\_Brigadier\_Tree\_CommandNode\_Children), 
[CommandNode.Name](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md\#Void\_Minecraft\_Commands\_Brigadier\_Tree\_CommandNode\_Name), 
[CommandNode.UsageText](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md\#Void\_Minecraft\_Commands\_Brigadier\_Tree\_CommandNode\_UsageText), 
[CommandNode.Examples](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md\#Void\_Minecraft\_Commands\_Brigadier\_Tree\_CommandNode\_Examples), 
[CommandNode.SortedKey](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md\#Void\_Minecraft\_Commands\_Brigadier\_Tree\_CommandNode\_SortedKey), 
[CommandNode.AddChild\(CommandNode\)](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md\#Void\_Minecraft\_Commands\_Brigadier\_Tree\_CommandNode\_AddChild\_Void\_Minecraft\_Commands\_Brigadier\_Tree\_CommandNode\_), 
[CommandNode.CanUseAsync\(ICommandSource, CancellationToken\)](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md\#Void\_Minecraft\_Commands\_Brigadier\_Tree\_CommandNode\_CanUseAsync\_Void\_Proxy\_Api\_Commands\_ICommandSource\_System\_Threading\_CancellationToken\_), 
[CommandNode.FindAmbiguities\(AmbiguousConsumer\)](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md\#Void\_Minecraft\_Commands\_Brigadier\_Tree\_CommandNode\_FindAmbiguities\_Void\_Minecraft\_Commands\_Brigadier\_AmbiguousConsumer\_), 
[CommandNode.GetChild\(string\)](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md\#Void\_Minecraft\_Commands\_Brigadier\_Tree\_CommandNode\_GetChild\_System\_String\_), 
[CommandNode.GetRelevantNodes\(StringReader\)](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md\#Void\_Minecraft\_Commands\_Brigadier\_Tree\_CommandNode\_GetRelevantNodes\_Void\_Minecraft\_Commands\_Brigadier\_StringReader\_), 
[CommandNode.CreateBuilder\(\)](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md\#Void\_Minecraft\_Commands\_Brigadier\_Tree\_CommandNode\_CreateBuilder), 
[CommandNode.IsValidInput\(string\)](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md\#Void\_Minecraft\_Commands\_Brigadier\_Tree\_CommandNode\_IsValidInput\_System\_String\_), 
[CommandNode.ListSuggestionsAsync\(CommandContext, SuggestionsBuilder, CancellationToken\)](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md\#Void\_Minecraft\_Commands\_Brigadier\_Tree\_CommandNode\_ListSuggestionsAsync\_Void\_Minecraft\_Commands\_Brigadier\_Context\_CommandContext\_Void\_Minecraft\_Commands\_Brigadier\_Suggestion\_SuggestionsBuilder\_System\_Threading\_CancellationToken\_), 
[CommandNode.Parse\(StringReader, CommandContextBuilder\)](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md\#Void\_Minecraft\_Commands\_Brigadier\_Tree\_CommandNode\_Parse\_Void\_Minecraft\_Commands\_Brigadier\_StringReader\_Void\_Minecraft\_Commands\_Brigadier\_Context\_CommandContextBuilder\_), 
[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Commands_Brigadier_Tree_Nodes_RootCommandNode__ctor"></a> RootCommandNode\(\)

```csharp
public RootCommandNode()
```

## Properties

### <a id="Void_Minecraft_Commands_Brigadier_Tree_Nodes_RootCommandNode_Examples"></a> Examples

```csharp
public override IEnumerable<string> Examples { get; }
```

#### Property Value

 [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[string](https://learn.microsoft.com/dotnet/api/system.string)\>

### <a id="Void_Minecraft_Commands_Brigadier_Tree_Nodes_RootCommandNode_Name"></a> Name

```csharp
public override string Name { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Commands_Brigadier_Tree_Nodes_RootCommandNode_SortedKey"></a> SortedKey

```csharp
protected override string SortedKey { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Commands_Brigadier_Tree_Nodes_RootCommandNode_UsageText"></a> UsageText

```csharp
public override string UsageText { get; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

## Methods

### <a id="Void_Minecraft_Commands_Brigadier_Tree_Nodes_RootCommandNode_CreateBuilder"></a> CreateBuilder\(\)

```csharp
public override IArgumentBuilder<CommandNode> CreateBuilder()
```

#### Returns

 [IArgumentBuilder](Void.Minecraft.Commands.Brigadier.Builder.IArgumentBuilder\-1.md)<[CommandNode](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md)\>

### <a id="Void_Minecraft_Commands_Brigadier_Tree_Nodes_RootCommandNode_IsValidInput_System_String_"></a> IsValidInput\(string\)

```csharp
public override bool IsValidInput(string input)
```

#### Parameters

`input` [string](https://learn.microsoft.com/dotnet/api/system.string)

#### Returns

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Commands_Brigadier_Tree_Nodes_RootCommandNode_ListSuggestionsAsync_Void_Minecraft_Commands_Brigadier_Context_CommandContext_Void_Minecraft_Commands_Brigadier_Suggestion_SuggestionsBuilder_System_Threading_CancellationToken_"></a> ListSuggestionsAsync\(CommandContext, SuggestionsBuilder, CancellationToken\)

```csharp
public override ValueTask<Suggestions> ListSuggestionsAsync(CommandContext context, SuggestionsBuilder builder, CancellationToken cancellationToken)
```

#### Parameters

`context` [CommandContext](Void.Minecraft.Commands.Brigadier.Context.CommandContext.md)

`builder` [SuggestionsBuilder](Void.Minecraft.Commands.Brigadier.Suggestion.SuggestionsBuilder.md)

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask\-1)<[Suggestions](Void.Minecraft.Commands.Brigadier.Suggestion.Suggestions.md)\>

### <a id="Void_Minecraft_Commands_Brigadier_Tree_Nodes_RootCommandNode_Parse_Void_Minecraft_Commands_Brigadier_StringReader_Void_Minecraft_Commands_Brigadier_Context_CommandContextBuilder_"></a> Parse\(StringReader, CommandContextBuilder\)

```csharp
public override void Parse(StringReader reader, CommandContextBuilder context)
```

#### Parameters

`reader` [StringReader](Void.Minecraft.Commands.Brigadier.StringReader.md)

`context` [CommandContextBuilder](Void.Minecraft.Commands.Brigadier.Context.CommandContextBuilder.md)

