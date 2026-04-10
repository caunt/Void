namespace Void.Proxy.Api.Commands;

/// <summary>
/// Describes the outcome of a command dispatched through <see cref="ICommandService.ExecuteAsync(ICommandSource, string, System.Threading.CancellationToken)" />.
/// </summary>
public enum CommandExecutionResult
{
    /// <summary>
    /// The command was not matched by any registered handler and was forwarded to the backend server.
    /// This outcome only occurs when the source is a player; a console source returns <see cref="Executed" /> for unrecognised input.
    /// </summary>
    Forwarded,

    /// <summary>
    /// The command was matched to a registered Brigadier node and dispatched for execution.
    /// </summary>
    Executed,

    /// <summary>
    /// Parsing the command input raised a <c>CommandSyntaxException</c>.
    /// When the source is a player the exception message is delivered to them before this value is returned.
    /// </summary>
    Exception
}
