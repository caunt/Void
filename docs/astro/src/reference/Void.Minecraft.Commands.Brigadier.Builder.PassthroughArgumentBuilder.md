# <a id="Void_Minecraft_Commands_Brigadier_Builder_PassthroughArgumentBuilder"></a> Class PassthroughArgumentBuilder

Namespace: [Void.Minecraft.Commands.Brigadier.Builder](Void.Minecraft.Commands.Brigadier.Builder.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record PassthroughArgumentBuilder : ArgumentBuilder<PassthroughArgumentBuilder, ArgumentCommandNode>, IEquatable<ArgumentBuilder>, IArgumentBuilder<ArgumentCommandNode>, IEquatable<ArgumentBuilder<PassthroughArgumentBuilder, ArgumentCommandNode>>, IEquatable<PassthroughArgumentBuilder>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ArgumentBuilder](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder.md) ← 
[ArgumentBuilder<PassthroughArgumentBuilder, ArgumentCommandNode\>](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder\-2.md) ← 
[PassthroughArgumentBuilder](Void.Minecraft.Commands.Brigadier.Builder.PassthroughArgumentBuilder.md)

#### Implements

[IEquatable<ArgumentBuilder\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1), 
[IArgumentBuilder<ArgumentCommandNode\>](Void.Minecraft.Commands.Brigadier.Builder.IArgumentBuilder\-1.md), 
[IEquatable<ArgumentBuilder<PassthroughArgumentBuilder, ArgumentCommandNode\>\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1), 
[IEquatable<PassthroughArgumentBuilder\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[ArgumentBuilder<PassthroughArgumentBuilder, ArgumentCommandNode\>.Then<TChildNode\>\(IArgumentBuilder<TChildNode\>\)](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder\-2.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_2\_Then\_\_1\_Void\_Minecraft\_Commands\_Brigadier\_Builder\_IArgumentBuilder\_\_\_0\_\_), 
[ArgumentBuilder<PassthroughArgumentBuilder, ArgumentCommandNode\>.Then<TChildNode\>\(CommandNode\)](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder\-2.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_2\_Then\_\_1\_Void\_Minecraft\_Commands\_Brigadier\_Tree\_CommandNode\_), 
[ArgumentBuilder<PassthroughArgumentBuilder, ArgumentCommandNode\>.Then<TChildNode\>\(Func<IArgumentContext, IArgumentBuilder<TChildNode\>\>\)](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder\-2.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_2\_Then\_\_1\_System\_Func\_Void\_Minecraft\_Commands\_Brigadier\_IArgumentContext\_Void\_Minecraft\_Commands\_Brigadier\_Builder\_IArgumentBuilder\_\_\_0\_\_\_), 
[ArgumentBuilder<PassthroughArgumentBuilder, ArgumentCommandNode\>.Executes\(CommandExecutor?\)](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder\-2.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_2\_Executes\_Void\_Minecraft\_Commands\_Brigadier\_CommandExecutor\_), 
[ArgumentBuilder<PassthroughArgumentBuilder, ArgumentCommandNode\>.Executes\(CommandExecutorSync?\)](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder\-2.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_2\_Executes\_Void\_Minecraft\_Commands\_Brigadier\_CommandExecutorSync\_), 
[ArgumentBuilder<PassthroughArgumentBuilder, ArgumentCommandNode\>.Requires\(CommandRequirement?\)](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder\-2.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_2\_Requires\_Void\_Minecraft\_Commands\_Brigadier\_Tree\_CommandRequirement\_), 
[ArgumentBuilder<PassthroughArgumentBuilder, ArgumentCommandNode\>.Redirect\(CommandNode\)](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder\-2.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_2\_Redirect\_Void\_Minecraft\_Commands\_Brigadier\_Tree\_CommandNode\_), 
[ArgumentBuilder<PassthroughArgumentBuilder, ArgumentCommandNode\>.Redirect\(CommandNode, SingleRedirectModifier\)](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder\-2.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_2\_Redirect\_Void\_Minecraft\_Commands\_Brigadier\_Tree\_CommandNode\_Void\_Minecraft\_Commands\_Brigadier\_SingleRedirectModifier\_), 
[ArgumentBuilder<PassthroughArgumentBuilder, ArgumentCommandNode\>.Fork\(CommandNode, RedirectModifier\)](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder\-2.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_2\_Fork\_Void\_Minecraft\_Commands\_Brigadier\_Tree\_CommandNode\_Void\_Minecraft\_Commands\_Brigadier\_RedirectModifier\_), 
[ArgumentBuilder<PassthroughArgumentBuilder, ArgumentCommandNode\>.Forward\(CommandNode?, RedirectModifier?, bool\)](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder\-2.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_2\_Forward\_Void\_Minecraft\_Commands\_Brigadier\_Tree\_CommandNode\_Void\_Minecraft\_Commands\_Brigadier\_RedirectModifier\_System\_Boolean\_), 
[ArgumentBuilder<PassthroughArgumentBuilder, ArgumentCommandNode\>.Suggests\(SuggestionProvider?\)](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder\-2.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_2\_Suggests\_Void\_Minecraft\_Commands\_Brigadier\_Suggestion\_SuggestionProvider\_), 
[ArgumentBuilder<PassthroughArgumentBuilder, ArgumentCommandNode\>.Build\(\)](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder\-2.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_2\_Build), 
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

## Constructors

### <a id="Void_Minecraft_Commands_Brigadier_Builder_PassthroughArgumentBuilder__ctor_Void_Minecraft_Commands_Brigadier_Registry_ArgumentSerializerMapping_System_String_Void_Minecraft_Commands_Brigadier_ArgumentTypes_IPassthroughArgumentValue_"></a> PassthroughArgumentBuilder\(ArgumentSerializerMapping, string, IPassthroughArgumentValue\)

```csharp
public PassthroughArgumentBuilder(ArgumentSerializerMapping Identifier, string Name, IPassthroughArgumentValue Result)
```

#### Parameters

`Identifier` [ArgumentSerializerMapping](Void.Minecraft.Commands.Brigadier.Registry.ArgumentSerializerMapping.md)

`Name` [string](https://learn.microsoft.com/dotnet/api/system.string)

`Result` [IPassthroughArgumentValue](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IPassthroughArgumentValue.md)

## Properties

### <a id="Void_Minecraft_Commands_Brigadier_Builder_PassthroughArgumentBuilder_Identifier"></a> Identifier

```csharp
public ArgumentSerializerMapping Identifier { get; init; }
```

#### Property Value

 [ArgumentSerializerMapping](Void.Minecraft.Commands.Brigadier.Registry.ArgumentSerializerMapping.md)

### <a id="Void_Minecraft_Commands_Brigadier_Builder_PassthroughArgumentBuilder_Name"></a> Name

```csharp
public string Name { get; init; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Commands_Brigadier_Builder_PassthroughArgumentBuilder_Result"></a> Result

```csharp
public IPassthroughArgumentValue Result { get; init; }
```

#### Property Value

 [IPassthroughArgumentValue](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IPassthroughArgumentValue.md)

### <a id="Void_Minecraft_Commands_Brigadier_Builder_PassthroughArgumentBuilder_SuggestionProvider"></a> SuggestionProvider

```csharp
public SuggestionProvider? SuggestionProvider { get; }
```

#### Property Value

 [SuggestionProvider](Void.Minecraft.Commands.Brigadier.Suggestion.SuggestionProvider.md)?

## Methods

### <a id="Void_Minecraft_Commands_Brigadier_Builder_PassthroughArgumentBuilder_Build"></a> Build\(\)

```csharp
public override ArgumentCommandNode Build()
```

#### Returns

 [ArgumentCommandNode](Void.Minecraft.Commands.Brigadier.Tree.Nodes.ArgumentCommandNode.md)

### <a id="Void_Minecraft_Commands_Brigadier_Builder_PassthroughArgumentBuilder_Suggests_Void_Minecraft_Commands_Brigadier_Suggestion_SuggestionProvider_"></a> Suggests\(SuggestionProvider?\)

```csharp
public override PassthroughArgumentBuilder Suggests(SuggestionProvider? provider)
```

#### Parameters

`provider` [SuggestionProvider](Void.Minecraft.Commands.Brigadier.Suggestion.SuggestionProvider.md)?

#### Returns

 [PassthroughArgumentBuilder](Void.Minecraft.Commands.Brigadier.Builder.PassthroughArgumentBuilder.md)

