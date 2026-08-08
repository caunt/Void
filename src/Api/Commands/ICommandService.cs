using Void.Proxy.Api.Network;

namespace Void.Proxy.Api.Commands;

/// <summary>
/// Provides access to command dispatching and execution operations.
/// </summary>
public interface ICommandService
{
    /// <summary>
    /// Gets the dispatcher used to route commands to their handlers.
    /// </summary>
    public ICommandDispatcher Dispatcher { get; }

    /// <summary>
    /// Executes a command on behalf of the specified <paramref name="source" />.
    /// </summary>
    /// <param name="source">The initiator of the command.</param>
    /// <param name="command">The command string to execute.</param>
    /// <param name="cancellationToken">Token used to cancel the execution.</param>
    /// <returns>A result describing the outcome of the command execution.</returns>
    public ValueTask<CommandExecutionResult> ExecuteAsync(ICommandSource source, string command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a command that originated from the specified <paramref name="origin" />.
    /// </summary>
    /// <param name="source">The initiator of the command.</param>
    /// <param name="command">The command string to execute.</param>
    /// <param name="origin">The side from which the command originated.</param>
    /// <param name="cancellationToken">Token used to cancel the execution.</param>
    /// <returns>A result describing the outcome of the command execution.</returns>
    public ValueTask<CommandExecutionResult> ExecuteAsync(ICommandSource source, string command, Side origin, CancellationToken cancellationToken = default);

    /// <summary>
    /// Completes the specified command <paramref name="input" /> using available commands.
    /// </summary>
    /// <param name="input">The partial command to complete.</param>
    /// <param name="source">The initiator requesting completion suggestions.</param>
    /// <param name="cancellationToken">Token used to cancel the completion operation.</param>
    /// <returns>An array of possible completions.</returns>
    public ValueTask<IEnumerable<ICommandSuggestion>> SuggestAsync(string input, ICommandSource source, CancellationToken cancellationToken = default);

    /// <summary>
    /// Copies commands available to a source into another root command node.
    /// </summary>
    /// <remarks>
    /// Existing destination children with the same names are replaced. Commands whose requirements reject <paramref name="commandSource" /> are omitted.
    /// </remarks>
    /// <param name="commandNode">The destination root command node.</param>
    /// <param name="commandSource">The source used to evaluate command requirements.</param>
    /// <param name="cancellationToken">A token used to cancel requirement evaluation.</param>
    /// <returns>A task that completes after all eligible commands have been copied.</returns>
    /// <exception cref="ArgumentException"><paramref name="commandNode" /> is not a root command node.</exception>
    public ValueTask CopyToAsync(ICommandNode commandNode, ICommandSource commandSource, CancellationToken cancellationToken = default);
}
