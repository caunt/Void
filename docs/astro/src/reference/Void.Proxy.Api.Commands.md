# <a id="Void_Proxy_Api_Commands"></a> Namespace Void.Proxy.Api.Commands

### Interfaces

 [ICommandDispatcher](Void.Proxy.Api.Commands.ICommandDispatcher.md)

Manages the Brigadier command tree by registering top-level command nodes.
The concrete implementation is <code>CommandDispatcher</code>, which roots all nodes under a single <code>RootCommandNode</code>.

 [ICommandNode](Void.Proxy.Api.Commands.ICommandNode.md)

 [ICommandService](Void.Proxy.Api.Commands.ICommandService.md)

Provides access to command dispatching and execution operations.

 [ICommandSource](Void.Proxy.Api.Commands.ICommandSource.md)

Represents an object that can act as the source of command execution and command completion requests.

### Enums

 [CommandExecutionResult](Void.Proxy.Api.Commands.CommandExecutionResult.md)

Describes the outcome of a command dispatched through <xref href="Void.Proxy.Api.Commands.ICommandService.ExecuteAsync(Void.Proxy.Api.Commands.ICommandSource%2cSystem.String%2cSystem.Threading.CancellationToken)" data-throw-if-not-resolved="false"></xref>.

