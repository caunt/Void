# <a id="Void_Minecraft_Commands_Brigadier_Builder_ArgumentBuilder_2"></a> Class ArgumentBuilder<TBuilder, TNode\>

Namespace: [Void.Minecraft.Commands.Brigadier.Builder](Void.Minecraft.Commands.Brigadier.Builder.md)  
Assembly: Void.Minecraft.dll  

```csharp
public abstract record ArgumentBuilder<TBuilder, TNode> : ArgumentBuilder, IEquatable<ArgumentBuilder>, IArgumentBuilder<TNode>, IEquatable<ArgumentBuilder<TBuilder, TNode>> where TBuilder : ArgumentBuilder<TBuilder, TNode> where TNode : CommandNode
```

#### Type Parameters

`TBuilder` 

`TNode` 

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ArgumentBuilder](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder.md) ← 
[ArgumentBuilder<TBuilder, TNode\>](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder\-2.md)

#### Implements

[IEquatable<ArgumentBuilder\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1), 
[IArgumentBuilder<TNode\>](Void.Minecraft.Commands.Brigadier.Builder.IArgumentBuilder\-1.md), 
[IEquatable<ArgumentBuilder<TBuilder, TNode\>\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[ArgumentBuilder.\_arguments](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_\_arguments), 
[ArgumentBuilder.Executor](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_Executor), 
[ArgumentBuilder.RedirectModifier](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_RedirectModifier), 
[ArgumentBuilder.RedirectTarget](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_RedirectTarget), 
[ArgumentBuilder.IsForks](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_IsForks), 
[ArgumentBuilder.Requirement](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_Requirement), 
[ArgumentBuilder.Arguments](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_Arguments), 
[ArgumentBuilder.Build\(\)](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_Build), 
[ArgumentBuilder.Executes\(CommandExecutor?\)](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_Executes\_Void\_Minecraft\_Commands\_Brigadier\_CommandExecutor\_), 
[ArgumentBuilder.Executes\(CommandExecutorSync?\)](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_Executes\_Void\_Minecraft\_Commands\_Brigadier\_CommandExecutorSync\_), 
[ArgumentBuilder.Requires\(CommandRequirement?\)](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_Requires\_Void\_Minecraft\_Commands\_Brigadier\_Tree\_CommandRequirement\_), 
[ArgumentBuilder.Redirect\(CommandNode\)](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_Redirect\_Void\_Minecraft\_Commands\_Brigadier\_Tree\_CommandNode\_), 
[ArgumentBuilder.Redirect\(CommandNode, SingleRedirectModifier\)](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_Redirect\_Void\_Minecraft\_Commands\_Brigadier\_Tree\_CommandNode\_Void\_Minecraft\_Commands\_Brigadier\_SingleRedirectModifier\_), 
[ArgumentBuilder.Fork\(CommandNode, RedirectModifier\)](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_Fork\_Void\_Minecraft\_Commands\_Brigadier\_Tree\_CommandNode\_Void\_Minecraft\_Commands\_Brigadier\_RedirectModifier\_), 
[ArgumentBuilder.Forward\(CommandNode?, RedirectModifier?, bool\)](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_Forward\_Void\_Minecraft\_Commands\_Brigadier\_Tree\_CommandNode\_Void\_Minecraft\_Commands\_Brigadier\_RedirectModifier\_System\_Boolean\_), 
[ArgumentBuilder.Suggests\(SuggestionProvider?\)](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_Suggests\_Void\_Minecraft\_Commands\_Brigadier\_Suggestion\_SuggestionProvider\_), 
[ArgumentBuilder.AddChild\(CommandNode\)](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_AddChild\_Void\_Minecraft\_Commands\_Brigadier\_Tree\_CommandNode\_), 
[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Methods

### <a id="Void_Minecraft_Commands_Brigadier_Builder_ArgumentBuilder_2_Build"></a> Build\(\)

```csharp
public override abstract TNode Build()
```

#### Returns

 TNode

### <a id="Void_Minecraft_Commands_Brigadier_Builder_ArgumentBuilder_2_Executes_Void_Minecraft_Commands_Brigadier_CommandExecutor_"></a> Executes\(CommandExecutor?\)

```csharp
public TBuilder Executes(CommandExecutor? command)
```

#### Parameters

`command` [CommandExecutor](Void.Minecraft.Commands.Brigadier.CommandExecutor.md)?

#### Returns

 TBuilder

### <a id="Void_Minecraft_Commands_Brigadier_Builder_ArgumentBuilder_2_Executes_Void_Minecraft_Commands_Brigadier_CommandExecutorSync_"></a> Executes\(CommandExecutorSync?\)

```csharp
public TBuilder Executes(CommandExecutorSync? command)
```

#### Parameters

`command` [CommandExecutorSync](Void.Minecraft.Commands.Brigadier.CommandExecutorSync.md)?

#### Returns

 TBuilder

### <a id="Void_Minecraft_Commands_Brigadier_Builder_ArgumentBuilder_2_Fork_Void_Minecraft_Commands_Brigadier_Tree_CommandNode_Void_Minecraft_Commands_Brigadier_RedirectModifier_"></a> Fork\(CommandNode, RedirectModifier\)

```csharp
public TBuilder Fork(CommandNode target, RedirectModifier modifier)
```

#### Parameters

`target` [CommandNode](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md)

`modifier` [RedirectModifier](Void.Minecraft.Commands.Brigadier.RedirectModifier.md)

#### Returns

 TBuilder

### <a id="Void_Minecraft_Commands_Brigadier_Builder_ArgumentBuilder_2_Forward_Void_Minecraft_Commands_Brigadier_Tree_CommandNode_Void_Minecraft_Commands_Brigadier_RedirectModifier_System_Boolean_"></a> Forward\(CommandNode?, RedirectModifier?, bool\)

```csharp
public TBuilder Forward(CommandNode? target, RedirectModifier? modifier, bool fork)
```

#### Parameters

`target` [CommandNode](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md)?

`modifier` [RedirectModifier](Void.Minecraft.Commands.Brigadier.RedirectModifier.md)?

`fork` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

#### Returns

 TBuilder

### <a id="Void_Minecraft_Commands_Brigadier_Builder_ArgumentBuilder_2_Redirect_Void_Minecraft_Commands_Brigadier_Tree_CommandNode_"></a> Redirect\(CommandNode\)

```csharp
public TBuilder Redirect(CommandNode target)
```

#### Parameters

`target` [CommandNode](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md)

#### Returns

 TBuilder

### <a id="Void_Minecraft_Commands_Brigadier_Builder_ArgumentBuilder_2_Redirect_Void_Minecraft_Commands_Brigadier_Tree_CommandNode_Void_Minecraft_Commands_Brigadier_SingleRedirectModifier_"></a> Redirect\(CommandNode, SingleRedirectModifier\)

```csharp
public TBuilder Redirect(CommandNode target, SingleRedirectModifier modifier)
```

#### Parameters

`target` [CommandNode](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md)

`modifier` [SingleRedirectModifier](Void.Minecraft.Commands.Brigadier.SingleRedirectModifier.md)

#### Returns

 TBuilder

### <a id="Void_Minecraft_Commands_Brigadier_Builder_ArgumentBuilder_2_Requires_Void_Minecraft_Commands_Brigadier_Tree_CommandRequirement_"></a> Requires\(CommandRequirement?\)

```csharp
public TBuilder Requires(CommandRequirement? requirement)
```

#### Parameters

`requirement` [CommandRequirement](Void.Minecraft.Commands.Brigadier.Tree.CommandRequirement.md)?

#### Returns

 TBuilder

### <a id="Void_Minecraft_Commands_Brigadier_Builder_ArgumentBuilder_2_Suggests_Void_Minecraft_Commands_Brigadier_Suggestion_SuggestionProvider_"></a> Suggests\(SuggestionProvider?\)

```csharp
public virtual TBuilder Suggests(SuggestionProvider? provider)
```

#### Parameters

`provider` [SuggestionProvider](Void.Minecraft.Commands.Brigadier.Suggestion.SuggestionProvider.md)?

#### Returns

 TBuilder

### <a id="Void_Minecraft_Commands_Brigadier_Builder_ArgumentBuilder_2_Then__1_Void_Minecraft_Commands_Brigadier_Builder_IArgumentBuilder___0__"></a> Then<TChildNode\>\(IArgumentBuilder<TChildNode\>\)

```csharp
public TBuilder Then<TChildNode>(IArgumentBuilder<TChildNode> argument) where TChildNode : CommandNode
```

#### Parameters

`argument` [IArgumentBuilder](Void.Minecraft.Commands.Brigadier.Builder.IArgumentBuilder\-1.md)<TChildNode\>

#### Returns

 TBuilder

#### Type Parameters

`TChildNode` 

### <a id="Void_Minecraft_Commands_Brigadier_Builder_ArgumentBuilder_2_Then__1_Void_Minecraft_Commands_Brigadier_Tree_CommandNode_"></a> Then<TChildNode\>\(CommandNode\)

```csharp
public TBuilder Then<TChildNode>(CommandNode node) where TChildNode : CommandNode
```

#### Parameters

`node` [CommandNode](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md)

#### Returns

 TBuilder

#### Type Parameters

`TChildNode` 

### <a id="Void_Minecraft_Commands_Brigadier_Builder_ArgumentBuilder_2_Then__1_System_Func_Void_Minecraft_Commands_Brigadier_IArgumentContext_Void_Minecraft_Commands_Brigadier_Builder_IArgumentBuilder___0___"></a> Then<TChildNode\>\(Func<IArgumentContext, IArgumentBuilder<TChildNode\>\>\)

```csharp
public TBuilder Then<TChildNode>(Func<IArgumentContext, IArgumentBuilder<TChildNode>> argument) where TChildNode : CommandNode
```

#### Parameters

`argument` [Func](https://learn.microsoft.com/dotnet/api/system.func\-2)<[IArgumentContext](Void.Minecraft.Commands.Brigadier.IArgumentContext.md), [IArgumentBuilder](Void.Minecraft.Commands.Brigadier.Builder.IArgumentBuilder\-1.md)<TChildNode\>\>

#### Returns

 TBuilder

#### Type Parameters

`TChildNode` 

