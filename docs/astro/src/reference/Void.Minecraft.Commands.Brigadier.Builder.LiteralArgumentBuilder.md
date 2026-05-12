# <a id="Void_Minecraft_Commands_Brigadier_Builder_LiteralArgumentBuilder"></a> Class LiteralArgumentBuilder

Namespace: [Void.Minecraft.Commands.Brigadier.Builder](Void.Minecraft.Commands.Brigadier.Builder.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record LiteralArgumentBuilder : ArgumentBuilder<LiteralArgumentBuilder, LiteralCommandNode>, IEquatable<ArgumentBuilder>, IArgumentBuilder<LiteralCommandNode>, IEquatable<ArgumentBuilder<LiteralArgumentBuilder, LiteralCommandNode>>, IEquatable<LiteralArgumentBuilder>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ArgumentBuilder](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder.md) ← 
[ArgumentBuilder<LiteralArgumentBuilder, LiteralCommandNode\>](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder\-2.md) ← 
[LiteralArgumentBuilder](Void.Minecraft.Commands.Brigadier.Builder.LiteralArgumentBuilder.md)

#### Implements

[IEquatable<ArgumentBuilder\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1), 
[IArgumentBuilder<LiteralCommandNode\>](Void.Minecraft.Commands.Brigadier.Builder.IArgumentBuilder\-1.md), 
[IEquatable<ArgumentBuilder<LiteralArgumentBuilder, LiteralCommandNode\>\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1), 
[IEquatable<LiteralArgumentBuilder\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[ArgumentBuilder<LiteralArgumentBuilder, LiteralCommandNode\>.Then<TChildNode\>\(IArgumentBuilder<TChildNode\>\)](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder\-2.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_2\_Then\_\_1\_Void\_Minecraft\_Commands\_Brigadier\_Builder\_IArgumentBuilder\_\_\_0\_\_), 
[ArgumentBuilder<LiteralArgumentBuilder, LiteralCommandNode\>.Then<TChildNode\>\(CommandNode\)](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder\-2.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_2\_Then\_\_1\_Void\_Minecraft\_Commands\_Brigadier\_Tree\_CommandNode\_), 
[ArgumentBuilder<LiteralArgumentBuilder, LiteralCommandNode\>.Then<TChildNode\>\(Func<IArgumentContext, IArgumentBuilder<TChildNode\>\>\)](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder\-2.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_2\_Then\_\_1\_System\_Func\_Void\_Minecraft\_Commands\_Brigadier\_IArgumentContext\_Void\_Minecraft\_Commands\_Brigadier\_Builder\_IArgumentBuilder\_\_\_0\_\_\_), 
[ArgumentBuilder<LiteralArgumentBuilder, LiteralCommandNode\>.Executes\(CommandExecutor?\)](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder\-2.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_2\_Executes\_Void\_Minecraft\_Commands\_Brigadier\_CommandExecutor\_), 
[ArgumentBuilder<LiteralArgumentBuilder, LiteralCommandNode\>.Executes\(CommandExecutorSync?\)](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder\-2.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_2\_Executes\_Void\_Minecraft\_Commands\_Brigadier\_CommandExecutorSync\_), 
[ArgumentBuilder<LiteralArgumentBuilder, LiteralCommandNode\>.Requires\(CommandRequirement?\)](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder\-2.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_2\_Requires\_Void\_Minecraft\_Commands\_Brigadier\_Tree\_CommandRequirement\_), 
[ArgumentBuilder<LiteralArgumentBuilder, LiteralCommandNode\>.Redirect\(CommandNode\)](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder\-2.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_2\_Redirect\_Void\_Minecraft\_Commands\_Brigadier\_Tree\_CommandNode\_), 
[ArgumentBuilder<LiteralArgumentBuilder, LiteralCommandNode\>.Redirect\(CommandNode, SingleRedirectModifier\)](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder\-2.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_2\_Redirect\_Void\_Minecraft\_Commands\_Brigadier\_Tree\_CommandNode\_Void\_Minecraft\_Commands\_Brigadier\_SingleRedirectModifier\_), 
[ArgumentBuilder<LiteralArgumentBuilder, LiteralCommandNode\>.Fork\(CommandNode, RedirectModifier\)](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder\-2.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_2\_Fork\_Void\_Minecraft\_Commands\_Brigadier\_Tree\_CommandNode\_Void\_Minecraft\_Commands\_Brigadier\_RedirectModifier\_), 
[ArgumentBuilder<LiteralArgumentBuilder, LiteralCommandNode\>.Forward\(CommandNode?, RedirectModifier?, bool\)](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder\-2.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_2\_Forward\_Void\_Minecraft\_Commands\_Brigadier\_Tree\_CommandNode\_Void\_Minecraft\_Commands\_Brigadier\_RedirectModifier\_System\_Boolean\_), 
[ArgumentBuilder<LiteralArgumentBuilder, LiteralCommandNode\>.Suggests\(SuggestionProvider?\)](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder\-2.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_2\_Suggests\_Void\_Minecraft\_Commands\_Brigadier\_Suggestion\_SuggestionProvider\_), 
[ArgumentBuilder<LiteralArgumentBuilder, LiteralCommandNode\>.Build\(\)](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder\-2.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_2\_Build), 
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

### <a id="Void_Minecraft_Commands_Brigadier_Builder_LiteralArgumentBuilder__ctor_System_String_"></a> LiteralArgumentBuilder\(string\)

```csharp
public LiteralArgumentBuilder(string Literal)
```

#### Parameters

`Literal` [string](https://learn.microsoft.com/dotnet/api/system.string)

## Properties

### <a id="Void_Minecraft_Commands_Brigadier_Builder_LiteralArgumentBuilder_Literal"></a> Literal

```csharp
public string Literal { get; init; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

## Methods

### <a id="Void_Minecraft_Commands_Brigadier_Builder_LiteralArgumentBuilder_Build"></a> Build\(\)

```csharp
public override LiteralCommandNode Build()
```

#### Returns

 [LiteralCommandNode](Void.Minecraft.Commands.Brigadier.Tree.Nodes.LiteralCommandNode.md)

### <a id="Void_Minecraft_Commands_Brigadier_Builder_LiteralArgumentBuilder_Create_System_String_"></a> Create\(string\)

```csharp
public static LiteralArgumentBuilder Create(string value)
```

#### Parameters

`value` [string](https://learn.microsoft.com/dotnet/api/system.string)

#### Returns

 [LiteralArgumentBuilder](Void.Minecraft.Commands.Brigadier.Builder.LiteralArgumentBuilder.md)

