# <a id="Void_Proxy_Api_Commands_CommandExecutionResult"></a> Enum CommandExecutionResult

Namespace: [Void.Proxy.Api.Commands](Void.Proxy.Api.Commands.md)  
Assembly: Void.Proxy.Api.dll  

Describes the outcome of a command dispatched through <xref href="Void.Proxy.Api.Commands.ICommandService.ExecuteAsync(Void.Proxy.Api.Commands.ICommandSource%2cSystem.String%2cSystem.Threading.CancellationToken)" data-throw-if-not-resolved="false"></xref>.

```csharp
public enum CommandExecutionResult
```

#### Extension Methods

[StructExtensions.IsDefault<CommandExecutionResult\>\(CommandExecutionResult\)](Void.Proxy.Api.Extensions.StructExtensions.md\#Void\_Proxy\_Api\_Extensions\_StructExtensions\_IsDefault\_\_1\_\_\_0\_)

## Fields

`Exception = 2` 

Parsing the command input raised a <code>CommandSyntaxException</code>.
When the source is a player the exception message is delivered to them before this value is returned.



`Executed = 1` 

The command was matched to a registered Brigadier node and dispatched for execution.



`Forwarded = 0` 

The command was not matched by any registered handler and was forwarded to the backend server.
This outcome only occurs when the source is a player; a console source returns <xref href="Void.Proxy.Api.Commands.CommandExecutionResult.Executed" data-throw-if-not-resolved="false"></xref> for unrecognised input.



