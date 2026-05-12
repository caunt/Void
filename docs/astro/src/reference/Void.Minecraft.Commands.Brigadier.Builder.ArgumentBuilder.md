# <a id="Void_Minecraft_Commands_Brigadier_Builder_ArgumentBuilder"></a> Class ArgumentBuilder

Namespace: [Void.Minecraft.Commands.Brigadier.Builder](Void.Minecraft.Commands.Brigadier.Builder.md)  
Assembly: Void.Minecraft.dll  

```csharp
public abstract record ArgumentBuilder : IEquatable<ArgumentBuilder>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ArgumentBuilder](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder.md)

#### Derived

[ArgumentBuilder<TBuilder, TNode\>](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder\-2.md)

#### Implements

[IEquatable<ArgumentBuilder\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Fields

### <a id="Void_Minecraft_Commands_Brigadier_Builder_ArgumentBuilder__arguments"></a> \_arguments

```csharp
protected readonly RootCommandNode _arguments
```

#### Field Value

 [RootCommandNode](Void.Minecraft.Commands.Brigadier.Tree.Nodes.RootCommandNode.md)

## Properties

### <a id="Void_Minecraft_Commands_Brigadier_Builder_ArgumentBuilder_Arguments"></a> Arguments

```csharp
public IEnumerable<CommandNode> Arguments { get; }
```

#### Property Value

 [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[CommandNode](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md)\>

### <a id="Void_Minecraft_Commands_Brigadier_Builder_ArgumentBuilder_Executor"></a> Executor

```csharp
public CommandExecutor? Executor { get; set; }
```

#### Property Value

 [CommandExecutor](Void.Minecraft.Commands.Brigadier.CommandExecutor.md)?

### <a id="Void_Minecraft_Commands_Brigadier_Builder_ArgumentBuilder_IsForks"></a> IsForks

```csharp
public bool IsForks { get; set; }
```

#### Property Value

 [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

### <a id="Void_Minecraft_Commands_Brigadier_Builder_ArgumentBuilder_RedirectModifier"></a> RedirectModifier

```csharp
public RedirectModifier? RedirectModifier { get; set; }
```

#### Property Value

 [RedirectModifier](Void.Minecraft.Commands.Brigadier.RedirectModifier.md)?

### <a id="Void_Minecraft_Commands_Brigadier_Builder_ArgumentBuilder_RedirectTarget"></a> RedirectTarget

```csharp
public CommandNode? RedirectTarget { get; set; }
```

#### Property Value

 [CommandNode](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md)?

### <a id="Void_Minecraft_Commands_Brigadier_Builder_ArgumentBuilder_Requirement"></a> Requirement

```csharp
public CommandRequirement? Requirement { get; set; }
```

#### Property Value

 [CommandRequirement](Void.Minecraft.Commands.Brigadier.Tree.CommandRequirement.md)?

## Methods

### <a id="Void_Minecraft_Commands_Brigadier_Builder_ArgumentBuilder_AddChild_Void_Minecraft_Commands_Brigadier_Tree_CommandNode_"></a> AddChild\(CommandNode\)

```csharp
protected void AddChild(CommandNode node)
```

#### Parameters

`node` [CommandNode](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md)

### <a id="Void_Minecraft_Commands_Brigadier_Builder_ArgumentBuilder_Build"></a> Build\(\)

```csharp
public abstract CommandNode Build()
```

#### Returns

 [CommandNode](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md)

### <a id="Void_Minecraft_Commands_Brigadier_Builder_ArgumentBuilder_Executes_Void_Minecraft_Commands_Brigadier_CommandExecutor_"></a> Executes\(CommandExecutor?\)

```csharp
public virtual ArgumentBuilder Executes(CommandExecutor? command)
```

#### Parameters

`command` [CommandExecutor](Void.Minecraft.Commands.Brigadier.CommandExecutor.md)?

#### Returns

 [ArgumentBuilder](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder.md)

### <a id="Void_Minecraft_Commands_Brigadier_Builder_ArgumentBuilder_Executes_Void_Minecraft_Commands_Brigadier_CommandExecutorSync_"></a> Executes\(CommandExecutorSync?\)

```csharp
public virtual ArgumentBuilder Executes(CommandExecutorSync? command)
```

#### Parameters

`command` [CommandExecutorSync](Void.Minecraft.Commands.Brigadier.CommandExecutorSync.md)?

#### Returns

 [ArgumentBuilder](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder.md)

### <a id="Void_Minecraft_Commands_Brigadier_Builder_ArgumentBuilder_Fork_Void_Minecraft_Commands_Brigadier_Tree_CommandNode_Void_Minecraft_Commands_Brigadier_RedirectModifier_"></a> Fork\(CommandNode, RedirectModifier\)

```csharp
public virtual ArgumentBuilder Fork(CommandNode target, RedirectModifier modifier)
```

#### Parameters

`target` [CommandNode](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md)

`modifier` [RedirectModifier](Void.Minecraft.Commands.Brigadier.RedirectModifier.md)

#### Returns

 [ArgumentBuilder](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder.md)

### <a id="Void_Minecraft_Commands_Brigadier_Builder_ArgumentBuilder_Forward_Void_Minecraft_Commands_Brigadier_Tree_CommandNode_Void_Minecraft_Commands_Brigadier_RedirectModifier_System_Boolean_"></a> Forward\(CommandNode?, RedirectModifier?, bool\)

```csharp
public virtual ArgumentBuilder Forward(CommandNode? target, RedirectModifier? modifier, bool fork)
```

#### Parameters

`target` [CommandNode](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md)?

`modifier` [RedirectModifier](Void.Minecraft.Commands.Brigadier.RedirectModifier.md)?

`fork` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

#### Returns

 [ArgumentBuilder](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder.md)

### <a id="Void_Minecraft_Commands_Brigadier_Builder_ArgumentBuilder_Redirect_Void_Minecraft_Commands_Brigadier_Tree_CommandNode_"></a> Redirect\(CommandNode\)

```csharp
public virtual ArgumentBuilder Redirect(CommandNode target)
```

#### Parameters

`target` [CommandNode](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md)

#### Returns

 [ArgumentBuilder](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder.md)

### <a id="Void_Minecraft_Commands_Brigadier_Builder_ArgumentBuilder_Redirect_Void_Minecraft_Commands_Brigadier_Tree_CommandNode_Void_Minecraft_Commands_Brigadier_SingleRedirectModifier_"></a> Redirect\(CommandNode, SingleRedirectModifier\)

```csharp
public virtual ArgumentBuilder Redirect(CommandNode target, SingleRedirectModifier modifier)
```

#### Parameters

`target` [CommandNode](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md)

`modifier` [SingleRedirectModifier](Void.Minecraft.Commands.Brigadier.SingleRedirectModifier.md)

#### Returns

 [ArgumentBuilder](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder.md)

### <a id="Void_Minecraft_Commands_Brigadier_Builder_ArgumentBuilder_Requires_Void_Minecraft_Commands_Brigadier_Tree_CommandRequirement_"></a> Requires\(CommandRequirement?\)

```csharp
public virtual ArgumentBuilder Requires(CommandRequirement? requirement)
```

#### Parameters

`requirement` [CommandRequirement](Void.Minecraft.Commands.Brigadier.Tree.CommandRequirement.md)?

#### Returns

 [ArgumentBuilder](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder.md)

### <a id="Void_Minecraft_Commands_Brigadier_Builder_ArgumentBuilder_Suggests_Void_Minecraft_Commands_Brigadier_Suggestion_SuggestionProvider_"></a> Suggests\(SuggestionProvider?\)

```csharp
public virtual ArgumentBuilder Suggests(SuggestionProvider? provider)
```

#### Parameters

`provider` [SuggestionProvider](Void.Minecraft.Commands.Brigadier.Suggestion.SuggestionProvider.md)?

#### Returns

 [ArgumentBuilder](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder.md)

