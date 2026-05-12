# <a id="Void_Minecraft_Commands_Brigadier_CommandExecutor"></a> Delegate CommandExecutor

Namespace: [Void.Minecraft.Commands.Brigadier](Void.Minecraft.Commands.Brigadier.md)  
Assembly: Void.Minecraft.dll  

```csharp
public delegate ValueTask<int> CommandExecutor(CommandContext context, CancellationToken cancellationToken)
```

#### Parameters

`context` [CommandContext](Void.Minecraft.Commands.Brigadier.Context.CommandContext.md)

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask\-1)<[int](https://learn.microsoft.com/dotnet/api/system.int32)\>

