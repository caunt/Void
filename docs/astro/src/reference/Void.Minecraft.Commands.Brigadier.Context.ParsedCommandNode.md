# <a id="Void_Minecraft_Commands_Brigadier_Context_ParsedCommandNode"></a> Class ParsedCommandNode

Namespace: [Void.Minecraft.Commands.Brigadier.Context](Void.Minecraft.Commands.Brigadier.Context.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record ParsedCommandNode : IEquatable<ParsedCommandNode>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ParsedCommandNode](Void.Minecraft.Commands.Brigadier.Context.ParsedCommandNode.md)

#### Implements

[IEquatable<ParsedCommandNode\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Commands_Brigadier_Context_ParsedCommandNode__ctor_Void_Minecraft_Commands_Brigadier_Tree_CommandNode_Void_Minecraft_Commands_Brigadier_Context_StringRange_"></a> ParsedCommandNode\(CommandNode, StringRange\)

```csharp
public ParsedCommandNode(CommandNode Node, StringRange Range)
```

#### Parameters

`Node` [CommandNode](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md)

`Range` [StringRange](Void.Minecraft.Commands.Brigadier.Context.StringRange.md)

## Properties

### <a id="Void_Minecraft_Commands_Brigadier_Context_ParsedCommandNode_Node"></a> Node

```csharp
public CommandNode Node { get; init; }
```

#### Property Value

 [CommandNode](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md)

### <a id="Void_Minecraft_Commands_Brigadier_Context_ParsedCommandNode_Range"></a> Range

```csharp
public StringRange Range { get; init; }
```

#### Property Value

 [StringRange](Void.Minecraft.Commands.Brigadier.Context.StringRange.md)

