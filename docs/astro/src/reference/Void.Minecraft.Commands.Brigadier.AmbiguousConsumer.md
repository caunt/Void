# <a id="Void_Minecraft_Commands_Brigadier_AmbiguousConsumer"></a> Delegate AmbiguousConsumer

Namespace: [Void.Minecraft.Commands.Brigadier](Void.Minecraft.Commands.Brigadier.md)  
Assembly: Void.Minecraft.dll  

```csharp
public delegate void AmbiguousConsumer(CommandNode parent, CommandNode children, CommandNode sibling, params IEnumerable<string> inputs)
```

#### Parameters

`parent` [CommandNode](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md)

`children` [CommandNode](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md)

`sibling` [CommandNode](Void.Minecraft.Commands.Brigadier.Tree.CommandNode.md)

`inputs` [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[string](https://learn.microsoft.com/dotnet/api/system.string)\>

