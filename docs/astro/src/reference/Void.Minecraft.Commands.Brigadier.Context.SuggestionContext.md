# <a id="Void_Minecraft_Commands_Brigadier_Context_SuggestionContext"></a> Class SuggestionContext

Namespace: [Void.Minecraft.Commands.Brigadier.Context](Void.Minecraft.Commands.Brigadier.Context.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record SuggestionContext : IEquatable<SuggestionContext>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[SuggestionContext](Void.Minecraft.Commands.Brigadier.Context.SuggestionContext.md)

#### Implements

[IEquatable<SuggestionContext\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Commands_Brigadier_Context_SuggestionContext__ctor_Void_Minecraft_Commands_Brigadier_Tree_CommandNode_System_Int32_"></a> SuggestionContext\(CommandNode, int\)

```csharp
public SuggestionContext(CommandNode Parent, int Start)
```

#### Parameters

`Parent` [CommandNode](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md)

`Start` [int](https://learn.microsoft.com/dotnet/api/system.int32)

## Properties

### <a id="Void_Minecraft_Commands_Brigadier_Context_SuggestionContext_Parent"></a> Parent

```csharp
public CommandNode Parent { get; init; }
```

#### Property Value

 [CommandNode](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md)

### <a id="Void_Minecraft_Commands_Brigadier_Context_SuggestionContext_Start"></a> Start

```csharp
public int Start { get; init; }
```

#### Property Value

 [int](https://learn.microsoft.com/dotnet/api/system.int32)

