# <a id="Void_Proxy_Api_Commands_ICommandService"></a> Interface ICommandService

Namespace: [Void.Proxy.Api.Commands](Void.Proxy.Api.Commands.md)  
Assembly: Void.Proxy.Api.dll  

Provides access to command dispatching and execution operations.

```csharp
public interface ICommandService
```

#### Extension Methods

[CommandServiceExtensions.Register\(ICommandService, Func<IArgumentContext, LiteralArgumentBuilder\>\)](Void.Minecraft.Commands.Brigadier.Extensions.CommandServiceExtensions.md\#Void\_Minecraft\_Commands\_Brigadier\_Extensions\_CommandServiceExtensions\_Register\_Void\_Proxy\_Api\_Commands\_ICommandService\_System\_Func\_Void\_Minecraft\_Commands\_Brigadier\_IArgumentContext\_Void\_Minecraft\_Commands\_Brigadier\_Builder\_LiteralArgumentBuilder\_\_)

## Properties

### <a id="Void_Proxy_Api_Commands_ICommandService_Dispatcher"></a> Dispatcher

Gets the dispatcher used to route commands to their handlers.

```csharp
ICommandDispatcher Dispatcher { get; }
```

#### Property Value

 [ICommandDispatcher](Void.Proxy.Api.Commands.ICommandDispatcher.md)

## Methods

### <a id="Void_Proxy_Api_Commands_ICommandService_CompleteAsync_System_String_Void_Proxy_Api_Commands_ICommandSource_System_Threading_CancellationToken_"></a> CompleteAsync\(string, ICommandSource, CancellationToken\)

Completes the specified command <code class="paramref">input</code> using available commands.

```csharp
ValueTask<string[]> CompleteAsync(string input, ICommandSource source, CancellationToken cancellationToken = default)
```

#### Parameters

`input` [string](https://learn.microsoft.com/dotnet/api/system.string)

The partial command to complete.

`source` [ICommandSource](Void.Proxy.Api.Commands.ICommandSource.md)

The initiator requesting completion suggestions.

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

Token used to cancel the completion operation.

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask\-1)<[string](https://learn.microsoft.com/dotnet/api/system.string)\[\]\>

An array of possible completions.

### <a id="Void_Proxy_Api_Commands_ICommandService_ExecuteAsync_Void_Proxy_Api_Commands_ICommandSource_System_String_System_Threading_CancellationToken_"></a> ExecuteAsync\(ICommandSource, string, CancellationToken\)

Executes a command on behalf of the specified <code class="paramref">source</code>.

```csharp
ValueTask<CommandExecutionResult> ExecuteAsync(ICommandSource source, string command, CancellationToken cancellationToken = default)
```

#### Parameters

`source` [ICommandSource](Void.Proxy.Api.Commands.ICommandSource.md)

The initiator of the command.

`command` [string](https://learn.microsoft.com/dotnet/api/system.string)

The command string to execute.

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

Token used to cancel the execution.

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask\-1)<[CommandExecutionResult](Void.Proxy.Api.Commands.CommandExecutionResult.md)\>

A result describing the outcome of the command execution.

### <a id="Void_Proxy_Api_Commands_ICommandService_ExecuteAsync_Void_Proxy_Api_Commands_ICommandSource_System_String_Void_Proxy_Api_Network_Side_System_Threading_CancellationToken_"></a> ExecuteAsync\(ICommandSource, string, Side, CancellationToken\)

Executes a command that originated from the specified <code class="paramref">origin</code>.

```csharp
ValueTask<CommandExecutionResult> ExecuteAsync(ICommandSource source, string command, Side origin, CancellationToken cancellationToken = default)
```

#### Parameters

`source` [ICommandSource](Void.Proxy.Api.Commands.ICommandSource.md)

The initiator of the command.

`command` [string](https://learn.microsoft.com/dotnet/api/system.string)

The command string to execute.

`origin` [Side](Void.Proxy.Api.Network.Side.md)

The side from which the command originated.

`cancellationToken` [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

Token used to cancel the execution.

#### Returns

 [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask\-1)<[CommandExecutionResult](Void.Proxy.Api.Commands.CommandExecutionResult.md)\>

A result describing the outcome of the command execution.

