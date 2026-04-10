namespace Void.Proxy.Api.Commands;

/// <summary>
/// Represents an object that can act as the source of command execution and command completion requests.
/// </summary>
/// <remarks>
/// Implementations are used by the command pipeline to evaluate permissions, execute handlers, and produce suggestions.
/// </remarks>
public interface ICommandSource;
