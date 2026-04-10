# <a id="Void_Minecraft_Commands_Brigadier_Tree_CommandRequirement"></a> Delegate CommandRequirement

Namespace: [Void.Minecraft.Commands.Brigadier.Tree](Void.Minecraft.Commands.Brigadier.Tree.md)  
Assembly: Void.Minecraft.dll  

```csharp
public delegate ValueTask<bool> CommandRequirement(ICommandSource source, CancellationToken cancellationToken)
```

#### Parameters

`source` [ICommandSource](Void.Proxy.Api.Commands.ICommandSource.md)

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask\-1)<[bool](https://learn.microsoft.com/dotnet/api/system.boolean)\>

