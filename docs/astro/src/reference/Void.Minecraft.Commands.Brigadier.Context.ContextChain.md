# <a id="Void_Minecraft_Commands_Brigadier_Context_ContextChain"></a> Class ContextChain

Namespace: [Void.Minecraft.Commands.Brigadier.Context](Void.Minecraft.Commands.Brigadier.Context.md)  
Assembly: Void.Minecraft.dll  

```csharp
public record ContextChain : IEquatable<ContextChain>
```

#### Inheritance

[object](https://learn.microsoft.com/dotnet/api/system.object) ← 
[ContextChain](Void.Minecraft.Commands.Brigadier.Context.ContextChain.md)

#### Implements

[IEquatable<ContextChain\>](https://learn.microsoft.com/dotnet/api/system.iequatable\-1)

#### Inherited Members

[object.Equals\(object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\)), 
[object.Equals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.equals\#system\-object\-equals\(system\-object\-system\-object\)), 
[object.GetHashCode\(\)](https://learn.microsoft.com/dotnet/api/system.object.gethashcode), 
[object.GetType\(\)](https://learn.microsoft.com/dotnet/api/system.object.gettype), 
[object.MemberwiseClone\(\)](https://learn.microsoft.com/dotnet/api/system.object.memberwiseclone), 
[object.ReferenceEquals\(object?, object?\)](https://learn.microsoft.com/dotnet/api/system.object.referenceequals), 
[object.ToString\(\)](https://learn.microsoft.com/dotnet/api/system.object.tostring)

## Constructors

### <a id="Void_Minecraft_Commands_Brigadier_Context_ContextChain__ctor_System_Collections_Generic_List_Void_Minecraft_Commands_Brigadier_Context_CommandContext__Void_Minecraft_Commands_Brigadier_Context_CommandContext_"></a> ContextChain\(List<CommandContext\>, CommandContext\)

```csharp
public ContextChain(List<CommandContext> Modifiers, CommandContext Executable)
```

#### Parameters

`Modifiers` [List](https://learn.microsoft.com/dotnet/api/system.collections.generic.list\-1)<[CommandContext](Void.Minecraft.Commands.Brigadier.Context.CommandContext.md)\>

`Executable` [CommandContext](Void.Minecraft.Commands.Brigadier.Context.CommandContext.md)

## Properties

### <a id="Void_Minecraft_Commands_Brigadier_Context_ContextChain_Executable"></a> Executable

```csharp
public CommandContext Executable { get; init; }
```

#### Property Value

 [CommandContext](Void.Minecraft.Commands.Brigadier.Context.CommandContext.md)

### <a id="Void_Minecraft_Commands_Brigadier_Context_ContextChain_Modifiers"></a> Modifiers

```csharp
public List<CommandContext> Modifiers { get; init; }
```

#### Property Value

 [List](https://learn.microsoft.com/dotnet/api/system.collections.generic.list\-1)<[CommandContext](Void.Minecraft.Commands.Brigadier.Context.CommandContext.md)\>

### <a id="Void_Minecraft_Commands_Brigadier_Context_ContextChain_NextStage"></a> NextStage

```csharp
public ContextChain? NextStage { get; }
```

#### Property Value

 [ContextChain](Void.Minecraft.Commands.Brigadier.Context.ContextChain.md)?

### <a id="Void_Minecraft_Commands_Brigadier_Context_ContextChain_Stage"></a> Stage

```csharp
public ContextChain.ContextChainStage Stage { get; }
```

#### Property Value

 [ContextChain](Void.Minecraft.Commands.Brigadier.Context.ContextChain.md).[ContextChainStage](Void.Minecraft.Commands.Brigadier.Context.ContextChain.ContextChainStage.md)

### <a id="Void_Minecraft_Commands_Brigadier_Context_ContextChain_TopContext"></a> TopContext

```csharp
public CommandContext TopContext { get; }
```

#### Property Value

 [CommandContext](Void.Minecraft.Commands.Brigadier.Context.CommandContext.md)

## Methods

### <a id="Void_Minecraft_Commands_Brigadier_Context_ContextChain_ExecuteAllAsync_Void_Proxy_Api_Commands_ICommandSource_Void_Minecraft_Commands_Brigadier_ResultConsumer_System_Threading_CancellationToken_"></a> ExecuteAllAsync\(ICommandSource, ResultConsumer, CancellationToken\)

```csharp
public ValueTask<int> ExecuteAllAsync(ICommandSource source, ResultConsumer resultConsumer, CancellationToken cancellationToken)
```

#### Parameters

`source` [ICommandSource](Void.Proxy.Api.Commands.ICommandSource.md)

`resultConsumer` [ResultConsumer](Void.Minecraft.Commands.Brigadier.ResultConsumer.md)

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask\-1)<[int](https://learn.microsoft.com/dotnet/api/system.int32)\>

### <a id="Void_Minecraft_Commands_Brigadier_Context_ContextChain_RunExecutableAsync_Void_Minecraft_Commands_Brigadier_Context_CommandContext_Void_Proxy_Api_Commands_ICommandSource_Void_Minecraft_Commands_Brigadier_ResultConsumer_System_Boolean_System_Threading_CancellationToken_"></a> RunExecutableAsync\(CommandContext, ICommandSource, ResultConsumer, bool, CancellationToken\)

```csharp
public static ValueTask<int> RunExecutableAsync(CommandContext context, ICommandSource source, ResultConsumer resultConsumer, bool forkedMode, CancellationToken cancellationToken)
```

#### Parameters

`context` [CommandContext](Void.Minecraft.Commands.Brigadier.Context.CommandContext.md)

`source` [ICommandSource](Void.Proxy.Api.Commands.ICommandSource.md)

`resultConsumer` [ResultConsumer](Void.Minecraft.Commands.Brigadier.ResultConsumer.md)

`forkedMode` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask\-1)<[int](https://learn.microsoft.com/dotnet/api/system.int32)\>

### <a id="Void_Minecraft_Commands_Brigadier_Context_ContextChain_RunModifier_Void_Minecraft_Commands_Brigadier_Context_CommandContext_Void_Proxy_Api_Commands_ICommandSource_Void_Minecraft_Commands_Brigadier_ResultConsumer_System_Boolean_"></a> RunModifier\(CommandContext, ICommandSource, ResultConsumer, bool\)

```csharp
public static IEnumerable<ICommandSource> RunModifier(CommandContext modifier, ICommandSource source, ResultConsumer resultConsumer, bool forkedMode)
```

#### Parameters

`modifier` [CommandContext](Void.Minecraft.Commands.Brigadier.Context.CommandContext.md)

`source` [ICommandSource](Void.Proxy.Api.Commands.ICommandSource.md)

`resultConsumer` [ResultConsumer](Void.Minecraft.Commands.Brigadier.ResultConsumer.md)

`forkedMode` [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

#### Returns

 [IEnumerable](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable\-1)<[ICommandSource](Void.Proxy.Api.Commands.ICommandSource.md)\>

### <a id="Void_Minecraft_Commands_Brigadier_Context_ContextChain_TryFlatten_Void_Minecraft_Commands_Brigadier_Context_CommandContext_"></a> TryFlatten\(CommandContext\)

```csharp
public static ContextChain? TryFlatten(CommandContext rootContext)
```

#### Parameters

`rootContext` [CommandContext](Void.Minecraft.Commands.Brigadier.Context.CommandContext.md)

#### Returns

 [ContextChain](Void.Minecraft.Commands.Brigadier.Context.ContextChain.md)?

