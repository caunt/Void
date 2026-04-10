namespace Void.Proxy.Api.Commands;

/// <summary>
/// Describes the outcome of a command execution attempt.
/// </summary>
public enum CommandExecutionResult
{
    /// <summary>
    /// The command was not recognized by the proxy dispatcher and was relayed to the
    /// connected backend server. Typically returned for player command sources.
    /// </summary>
    Forwarded,

    /// <summary>
    /// The command was recognized by the proxy dispatcher and dispatched for execution.
    /// </summary>
    /// <remarks>
    /// Also returned for non-player sources when the command is not recognized, because
    /// non-player sources cannot be forwarded to a backend server.
    /// </remarks>
    Executed,

    /// <summary>
    /// A syntax error occurred while parsing the command string. If the source is a player,
    /// the error message is sent to them directly.
    /// </summary>
    Exception
}
