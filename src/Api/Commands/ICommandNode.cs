namespace Void.Proxy.Api.Commands;

/// <summary>
/// Represents a node in the command tree used by the proxy command APIs.
/// </summary>
/// <remarks>
/// Implementations are passed to <see cref="ICommandDispatcher.Add(ICommandNode)" /> to register commands and to
/// <see cref="ICommandService.CopyToAsync(ICommandNode, ICommandSource, CancellationToken)" /> to expose a command tree
/// to a command source.
/// </remarks>
public interface ICommandNode;
