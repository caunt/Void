# <a id="Void_Minecraft_Commands_Brigadier_Builder_RequiredArgumentBuilder"></a> Class RequiredArgumentBuilder

Namespace: [Void.Minecraft.Commands.Brigadier.Builder](Void.Minecraft.Commands.Brigadier.Builder.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record RequiredArgumentBuilder : ArgumentBuilder<RequiredArgumentBuilder, ArgumentCommandNode>, IEquatable<ArgumentBuilder>, IArgumentBuilder<ArgumentCommandNode>, IEquatable<ArgumentBuilder<RequiredArgumentBuilder, ArgumentCommandNode>>, IEquatable<RequiredArgumentBuilder>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ArgumentBuilder](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder.md) ← 
[ArgumentBuilder<RequiredArgumentBuilder, ArgumentCommandNode\>](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder\-2.md) ← 
[RequiredArgumentBuilder](Void.Minecraft.Commands.Brigadier.Builder.RequiredArgumentBuilder.md)

#### Implements

[IEquatable<ArgumentBuilder\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1), 
[IArgumentBuilder<ArgumentCommandNode\>](Void.Minecraft.Commands.Brigadier.Builder.IArgumentBuilder\-1.md), 
[IEquatable<ArgumentBuilder<RequiredArgumentBuilder, ArgumentCommandNode\>\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1), 
[IEquatable<RequiredArgumentBuilder\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[ArgumentBuilder<RequiredArgumentBuilder, ArgumentCommandNode\>.Then<TChildNode\>\(IArgumentBuilder<TChildNode\>\)](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder\-2.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_2\_Then\_\_1\_Void\_Minecraft\_Commands\_Brigadier\_Builder\_IArgumentBuilder\_\_\_0\_\_), 
[ArgumentBuilder<RequiredArgumentBuilder, ArgumentCommandNode\>.Then<TChildNode\>\(CommandNode\)](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder\-2.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_2\_Then\_\_1\_Void\_Minecraft\_Commands\_Brigadier\_Tree\_CommandNode\_), 
[ArgumentBuilder<RequiredArgumentBuilder, ArgumentCommandNode\>.Then<TChildNode\>\(Func<IArgumentContext, IArgumentBuilder<TChildNode\>\>\)](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder\-2.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_2\_Then\_\_1\_System\_Func\_Void\_Minecraft\_Commands\_Brigadier\_IArgumentContext\_Void\_Minecraft\_Commands\_Brigadier\_Builder\_IArgumentBuilder\_\_\_0\_\_\_), 
[ArgumentBuilder<RequiredArgumentBuilder, ArgumentCommandNode\>.Executes\(CommandExecutor?\)](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder\-2.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_2\_Executes\_Void\_Minecraft\_Commands\_Brigadier\_CommandExecutor\_), 
[ArgumentBuilder<RequiredArgumentBuilder, ArgumentCommandNode\>.Executes\(CommandExecutorSync?\)](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder\-2.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_2\_Executes\_Void\_Minecraft\_Commands\_Brigadier\_CommandExecutorSync\_), 
[ArgumentBuilder<RequiredArgumentBuilder, ArgumentCommandNode\>.Requires\(CommandRequirement?\)](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder\-2.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_2\_Requires\_Void\_Minecraft\_Commands\_Brigadier\_Tree\_CommandRequirement\_), 
[ArgumentBuilder<RequiredArgumentBuilder, ArgumentCommandNode\>.Redirect\(CommandNode\)](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder\-2.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_2\_Redirect\_Void\_Minecraft\_Commands\_Brigadier\_Tree\_CommandNode\_), 
[ArgumentBuilder<RequiredArgumentBuilder, ArgumentCommandNode\>.Redirect\(CommandNode, SingleRedirectModifier\)](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder\-2.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_2\_Redirect\_Void\_Minecraft\_Commands\_Brigadier\_Tree\_CommandNode\_Void\_Minecraft\_Commands\_Brigadier\_SingleRedirectModifier\_), 
[ArgumentBuilder<RequiredArgumentBuilder, ArgumentCommandNode\>.Fork\(CommandNode, RedirectModifier\)](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder\-2.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_2\_Fork\_Void\_Minecraft\_Commands\_Brigadier\_Tree\_CommandNode\_Void\_Minecraft\_Commands\_Brigadier\_RedirectModifier\_), 
[ArgumentBuilder<RequiredArgumentBuilder, ArgumentCommandNode\>.Forward\(CommandNode?, RedirectModifier?, bool\)](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder\-2.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_2\_Forward\_Void\_Minecraft\_Commands\_Brigadier\_Tree\_CommandNode\_Void\_Minecraft\_Commands\_Brigadier\_RedirectModifier\_System\_Boolean\_), 
[ArgumentBuilder<RequiredArgumentBuilder, ArgumentCommandNode\>.Suggests\(SuggestionProvider?\)](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder\-2.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_2\_Suggests\_Void\_Minecraft\_Commands\_Brigadier\_Suggestion\_SuggestionProvider\_), 
[ArgumentBuilder<RequiredArgumentBuilder, ArgumentCommandNode\>.Build\(\)](Void.Minecraft.Commands.Brigadier.Builder.ArgumentBuilder\-2.md\#Void\_Minecraft\_Commands\_Brigadier\_Builder\_ArgumentBuilder\_2\_Build), 
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

### <a id="Void_Minecraft_Commands_Brigadier_Builder_RequiredArgumentBuilder__ctor_System_String_Void_Minecraft_Commands_Brigadier_ArgumentTypes_IArgumentType_"></a> RequiredArgumentBuilder\(string, IArgumentType\)

```csharp
public RequiredArgumentBuilder(string Name, IArgumentType Type)
```

#### Parameters

`Name` [string](https://learn.microsoft.com/dotnet/api/system.string)

`Type` [IArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IArgumentType.md)

## Properties

### <a id="Void_Minecraft_Commands_Brigadier_Builder_RequiredArgumentBuilder_Name"></a> Name

```csharp
public string Name { get; init; }
```

#### Property Value

 [string](https://learn.microsoft.com/dotnet/api/system.string)

### <a id="Void_Minecraft_Commands_Brigadier_Builder_RequiredArgumentBuilder_SuggestionProvider"></a> SuggestionProvider

```csharp
public SuggestionProvider? SuggestionProvider { get; }
```

#### Property Value

 [SuggestionProvider](Void.Minecraft.Commands.Brigadier.Suggestion.SuggestionProvider.md)?

### <a id="Void_Minecraft_Commands_Brigadier_Builder_RequiredArgumentBuilder_Type"></a> Type

```csharp
public IArgumentType Type { get; init; }
```

#### Property Value

 [IArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IArgumentType.md)

## Methods

### <a id="Void_Minecraft_Commands_Brigadier_Builder_RequiredArgumentBuilder_Build"></a> Build\(\)

```csharp
public override ArgumentCommandNode Build()
```

#### Returns

 [ArgumentCommandNode](Void.Minecraft.Commands.Brigadier.Tree.Nodes.ArgumentCommandNode.md)

### <a id="Void_Minecraft_Commands_Brigadier_Builder_RequiredArgumentBuilder_Create_System_String_Void_Minecraft_Commands_Brigadier_ArgumentTypes_IArgumentType_"></a> Create\(string, IArgumentType\)

```csharp
public static RequiredArgumentBuilder Create(string name, IArgumentType type)
```

#### Parameters

`name` [string](https://learn.microsoft.com/dotnet/api/system.string)

`type` [IArgumentType](Void.Minecraft.Commands.Brigadier.ArgumentTypes.IArgumentType.md)

#### Returns

 [RequiredArgumentBuilder](Void.Minecraft.Commands.Brigadier.Builder.RequiredArgumentBuilder.md)

### <a id="Void_Minecraft_Commands_Brigadier_Builder_RequiredArgumentBuilder_Suggests_Void_Minecraft_Commands_Brigadier_Suggestion_SuggestionProvider_"></a> Suggests\(SuggestionProvider?\)

```csharp
public override RequiredArgumentBuilder Suggests(SuggestionProvider? provider)
```

#### Parameters

`provider` [SuggestionProvider](Void.Minecraft.Commands.Brigadier.Suggestion.SuggestionProvider.md)?

#### Returns

 [RequiredArgumentBuilder](Void.Minecraft.Commands.Brigadier.Builder.RequiredArgumentBuilder.md)

### <a id="Void_Minecraft_Commands_Brigadier_Builder_RequiredArgumentBuilder_Suggests_Void_Minecraft_Commands_Brigadier_Suggestion_SuggestionProviderSync_"></a> Suggests\(SuggestionProviderSync?\)

```csharp
public RequiredArgumentBuilder Suggests(SuggestionProviderSync? provider)
```

#### Parameters

`provider` [SuggestionProviderSync](Void.Minecraft.Commands.Brigadier.Suggestion.SuggestionProviderSync.md)?

#### Returns

 [RequiredArgumentBuilder](Void.Minecraft.Commands.Brigadier.Builder.RequiredArgumentBuilder.md)

